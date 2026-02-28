using FlashMediator;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Tenant.Application.Features.CQRS.Subscription.Exceptions;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.Subscription.Command.CreateStripeCheckoutSession
{
    public sealed class CreateStripeCheckoutSessionCommandHandler : IRequestHandler<CreateStripeCheckoutSessionCommandRequest, CreateStripeCheckoutSessionCommandResponse>
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly IConfiguration _configuration;

        public CreateStripeCheckoutSessionCommandHandler(
            ITenantReadRepository tenantReadRepository,
            IConfiguration configuration)
        {
            _tenantReadRepository = tenantReadRepository;
            _configuration = configuration;
        }

        public async Task<CreateStripeCheckoutSessionCommandResponse> Handle(CreateStripeCheckoutSessionCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.CompanyId), "CompanyId zorunludur.") });
            }

            if (string.IsNullOrWhiteSpace(request.PlanSlug) && string.IsNullOrWhiteSpace(request.PlanName))
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.PlanName), "Plan bilgisi zorunludur.") });
            }

            var selectedPlan = await _tenantReadRepository.GetActivePlanByKey(request.PlanSlug, request.PlanName, cancellationToken);
            if (selectedPlan is null)
            {
                throw new CompanyPlanNotFoundExceptions();
            }

            if (selectedPlan.PlanPrice <= 0)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(selectedPlan.PlanPrice), "Ucretsiz plan icin Stripe odemesi olusturulamaz.") });
            }

            var normalizedPlanKey = NormalizePlanKey(request.PlanSlug, selectedPlan.PlanName);
            var clientReferenceId = BuildClientReferenceId(request.CompanyId, normalizedPlanKey);
            var stripeSecretKey =
                _configuration["stripe_secret_key"]
                ?? Environment.GetEnvironmentVariable("TF_STRIPE_SECRET_KEY")
                ?? Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                ?? _configuration["Stripe:SecretKey"];
            stripeSecretKey = stripeSecretKey?.Trim();

            if (string.IsNullOrWhiteSpace(stripeSecretKey))
            {
                throw new InvalidOperationException("Stripe secret key tanimlanmamis.");
            }

            if (!stripeSecretKey.StartsWith("sk_", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Stripe secret key gecersiz. Secret key 'sk_' ile baslamali.");
            }

            var defaultSuccessUrl = _configuration["Stripe:SuccessUrl"] ?? "http://localhost:5173/subscription/payment-success";
            var defaultCancelUrl = _configuration["Stripe:CancelUrl"] ?? "http://localhost:5173/subscription/payment-success";

            var successUrlBase = string.IsNullOrWhiteSpace(request.SuccessUrl) ? defaultSuccessUrl : request.SuccessUrl.Trim();
            var cancelUrlBase = string.IsNullOrWhiteSpace(request.CancelUrl) ? defaultCancelUrl : request.CancelUrl.Trim();

            var successUrl = EnsureCheckoutReturnUrl(
                successUrlBase,
                paymentStatus: "success",
                request.CompanyId,
                selectedPlan.PlanName,
                normalizedPlanKey,
                includeSessionIdPlaceholder: true);

            var cancelUrl = EnsureCheckoutReturnUrl(
                cancelUrlBase,
                paymentStatus: "cancel",
                request.CompanyId,
                selectedPlan.PlanName,
                normalizedPlanKey,
                includeSessionIdPlaceholder: false);

            var formValues = new Dictionary<string, string>
            {
                ["mode"] = "subscription",
                ["success_url"] = successUrl,
                ["cancel_url"] = cancelUrl,
                ["client_reference_id"] = clientReferenceId,
                ["line_items[0][quantity]"] = "1",
                ["line_items[0][price_data][currency]"] = "try",
                ["line_items[0][price_data][unit_amount]"] = (selectedPlan.PlanPrice * 100).ToString(CultureInfo.InvariantCulture),
                ["line_items[0][price_data][recurring][interval]"] = "month",
                ["line_items[0][price_data][product_data][name]"] = $"TaskFlow {selectedPlan.PlanName}",
                ["metadata[plan_name]"] = selectedPlan.PlanName,
                ["metadata[plan_slug]"] = normalizedPlanKey,
                ["metadata[plan_price]"] = selectedPlan.PlanPrice.ToString(CultureInfo.InvariantCulture),
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stripeSecretKey);

            using var stripeRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.stripe.com/v1/checkout/sessions")
            {
                Content = new FormUrlEncodedContent(formValues)
            };

            using var stripeResponse = await httpClient.SendAsync(stripeRequest, cancellationToken);
            var responseBody = await stripeResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!stripeResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Stripe checkout session olusturulamadi. Detail: {responseBody}");
            }

            using var responseJson = JsonDocument.Parse(responseBody);
            var root = responseJson.RootElement;

            var checkoutUrl = root.TryGetProperty("url", out var urlElement) ? urlElement.GetString() : null;
            var sessionId = root.TryGetProperty("id", out var idElement) ? idElement.GetString() : null;

            if (string.IsNullOrWhiteSpace(checkoutUrl))
            {
                throw new InvalidOperationException("Stripe checkout URL donmedi.");
            }

            return new CreateStripeCheckoutSessionCommandResponse
            {
                SessionId = sessionId,
                CheckoutUrl = checkoutUrl,
                PlanName = selectedPlan.PlanName,
                PlanPrice = selectedPlan.PlanPrice
            };
        }

        private static string BuildClientReferenceId(Guid companyId, string normalizedPlanKey)
        {
            var safePlanKey = string.IsNullOrWhiteSpace(normalizedPlanKey)
                ? "unknown"
                : normalizedPlanKey.Trim().ToLowerInvariant();

            return $"{companyId:D}__{safePlanKey}";
        }

        private static string EnsureCheckoutReturnUrl(
            string baseUrl,
            string paymentStatus,
            Guid companyId,
            string planName,
            string planSlug,
            bool includeSessionIdPlaceholder)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return string.Empty;
            }

            var url = baseUrl.Trim();
            url = AppendQueryIfMissing(url, "payment", paymentStatus);
            url = AppendQueryIfMissing(url, "companyId", companyId.ToString("D"));
            url = AppendQueryIfMissing(url, "plan", planName);
            url = AppendQueryIfMissing(url, "slug", planSlug);
            if (includeSessionIdPlaceholder)
            {
                url = AppendQueryIfMissing(url, "session_id", "{CHECKOUT_SESSION_ID}");
            }

            return url;
        }

        private static string AppendQueryIfMissing(string url, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return url;
            }

            if (url.Contains($"{key}=", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            var separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
        }

        private static string NormalizePlanKey(string? planSlug, string planName)
        {
            var source = (string.IsNullOrWhiteSpace(planSlug) ? planName : planSlug) ?? string.Empty;
            var normalized = source.Trim().ToLowerInvariant();
            if (normalized.Contains("start"))
            {
                return "startup";
            }
            if (normalized.Contains("business") || normalized.Contains("profesyonel"))
            {
                return "business";
            }
            if (normalized.Contains("enterprise") || normalized.Contains("kurumsal"))
            {
                return "enterprise";
            }

            return normalized;
        }

        private static string? FirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return null;
        }
    }
}

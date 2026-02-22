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
            if (string.IsNullOrWhiteSpace(request.PlanSlug) && string.IsNullOrWhiteSpace(request.PlanName))
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.PlanName), "Plan bilgisi zorunludur.") });
            }

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

            var selectedPlan = await _tenantReadRepository.GetActivePlanByKey(request.PlanSlug, request.PlanName, cancellationToken);
            if (selectedPlan is null)
            {
                throw new CompanyPlanNotFoundExceptions();
            }

            if (selectedPlan.PlanPrice <= 0)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(selectedPlan.PlanPrice), "Ucretsiz plan icin Stripe odemesi olusturulamaz.") });
            }

            var defaultSuccessUrl = _configuration["Stripe:SuccessUrl"] ?? "http://localhost:5173/checkout?status=success";
            var defaultCancelUrl = _configuration["Stripe:CancelUrl"] ?? "http://localhost:5173/checkout?status=cancel";

            var successUrl = string.IsNullOrWhiteSpace(request.SuccessUrl) ? defaultSuccessUrl : request.SuccessUrl.Trim();
            var cancelUrl = string.IsNullOrWhiteSpace(request.CancelUrl) ? defaultCancelUrl : request.CancelUrl.Trim();

            var formValues = new Dictionary<string, string>
            {
                ["mode"] = "subscription",
                ["success_url"] = successUrl,
                ["cancel_url"] = cancelUrl,
                ["line_items[0][quantity]"] = "1",
                ["line_items[0][price_data][currency]"] = "try",
                ["line_items[0][price_data][unit_amount]"] = (selectedPlan.PlanPrice * 100).ToString(CultureInfo.InvariantCulture),
                ["line_items[0][price_data][recurring][interval]"] = "month",
                ["line_items[0][price_data][product_data][name]"] = $"TaskFlow {selectedPlan.PlanName}",
                ["metadata[plan_name]"] = selectedPlan.PlanName,
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
    }
}

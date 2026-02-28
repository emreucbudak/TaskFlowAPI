using FlashMediator;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using TaskFlow.BuildingBlocks.Enums;
using Tenant.Application.Features.CQRS.Subscription.Command.Activate;
using Tenant.Application.Features.CQRS.Subscription.Exceptions;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.Subscription.Command.ConfirmStripePaymentAndActivate;

public sealed class ConfirmStripePaymentAndActivateCommandHandler : IRequestHandler<ConfirmStripePaymentAndActivateCommandRequest, ConfirmStripePaymentAndActivateCommandResponse>
{
    private readonly IConfiguration _configuration;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ITenantWriteRepository _tenantWriteRepository;
    private readonly IMediator _mediator;

    public ConfirmStripePaymentAndActivateCommandHandler(
        IConfiguration configuration,
        ITenantReadRepository tenantReadRepository,
        ITenantWriteRepository tenantWriteRepository,
        IMediator mediator)
    {
        _configuration = configuration;
        _tenantReadRepository = tenantReadRepository;
        _tenantWriteRepository = tenantWriteRepository;
        _mediator = mediator;
    }

    public async Task<ConfirmStripePaymentAndActivateCommandResponse> Handle(
        ConfirmStripePaymentAndActivateCommandRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.SessionId), "SessionId zorunludur.") });
        }

        var stripeSecretKey = ResolveStripeSecretKey();
        if (string.IsNullOrWhiteSpace(stripeSecretKey))
        {
            throw new InvalidOperationException("Stripe secret key tanimlanmamis.");
        }

        if (!stripeSecretKey.StartsWith("sk_", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Stripe secret key gecersiz. Secret key 'sk_' ile baslamali.");
        }

        var normalizedRequestedSessionId = request.SessionId.Trim();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", stripeSecretKey);

        var stripeUrl = $"https://api.stripe.com/v1/checkout/sessions/{Uri.EscapeDataString(normalizedRequestedSessionId)}";
        using var stripeRequest = new HttpRequestMessage(HttpMethod.Get, stripeUrl);
        using var stripeResponse = await httpClient.SendAsync(stripeRequest, cancellationToken);
        var responseBody = await stripeResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!stripeResponse.IsSuccessStatusCode)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SessionId), $"Stripe session dogrulanamadi. Detail: {responseBody}")
            });
        }

        using var responseJson = JsonDocument.Parse(responseBody);
        var root = responseJson.RootElement;

        var stripeSessionId = TryReadString(root, "id")?.Trim();
        if (!string.Equals(stripeSessionId, normalizedRequestedSessionId, StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SessionId), "Stripe session id eslesmiyor.")
            });
        }

        var paymentStatus = TryReadString(root, "payment_status");
        var checkoutStatus = TryReadString(root, "status");
        var mode = TryReadString(root, "mode");
        var isPaid = string.Equals(paymentStatus, "paid", StringComparison.OrdinalIgnoreCase)
            && string.Equals(checkoutStatus, "complete", StringComparison.OrdinalIgnoreCase);

        if (!isPaid)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SessionId), "Stripe odemesi tamamlanmamis.")
            });
        }

        if (!string.Equals(mode, "subscription", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SessionId), $"Stripe checkout mode gecersiz: {mode ?? "null"}")
            });
        }

        var clientReferenceId = TryReadString(root, "client_reference_id");
        var hasClientReferenceId = TryParseClientReferenceId(clientReferenceId, out var companyIdFromStripe, out var planSlugFromStripe);

        var resolvedCompanyId = request.CompanyId;
        if (hasClientReferenceId)
        {
            if (request.CompanyId != Guid.Empty && companyIdFromStripe != request.CompanyId)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.CompanyId), "Session CompanyId bilgisi ile istek CompanyId eslesmiyor.")
                });
            }

            resolvedCompanyId = companyIdFromStripe;
        }
        else if (request.CompanyId == Guid.Empty)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.CompanyId), "CompanyId bilgisi request veya Stripe session icinde bulunamadi.")
            });
        }

        string? planNameFromMetadata = null;
        string? planSlugFromMetadata = null;
        if (TryGetProperty(root, "metadata", out var metadataElement))
        {
            planNameFromMetadata = TryReadString(metadataElement, "plan_name");
            planSlugFromMetadata = TryReadString(metadataElement, "plan_slug");
        }

        var resolvedPlanSlug = FirstNonEmpty(request.PlanSlug, planSlugFromMetadata, planSlugFromStripe);
        var resolvedPlanName = FirstNonEmpty(request.PlanName, planNameFromMetadata);
        if (string.IsNullOrWhiteSpace(resolvedPlanSlug) && string.IsNullOrWhiteSpace(resolvedPlanName))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.PlanSlug), "Plan bilgisi Stripe session icinde bulunamadi.")
            });
        }

        var selectedPlan = await _tenantReadRepository.GetActivePlanByKey(resolvedPlanSlug, resolvedPlanName, cancellationToken);
        if (selectedPlan is null)
        {
            throw new CompanyPlanNotFoundExceptions();
        }

        var expectedPlanSlug = NormalizePlanKey(resolvedPlanSlug, selectedPlan.PlanName);
        if (!string.IsNullOrWhiteSpace(planSlugFromStripe)
            && !string.Equals(planSlugFromStripe, expectedPlanSlug, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.PlanSlug), "Stripe session plan bilgisi ile secilen plan uyusmuyor.")
            });
        }

        var amountTotal = TryReadLong(root, "amount_total");
        if (amountTotal.HasValue)
        {
            var expectedAmount = (long)selectedPlan.PlanPrice * 100L;
            if (amountTotal.Value != expectedAmount)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.SessionId), "Stripe odeme tutari secilen plan ile uyusmuyor.")
                });
            }
        }

        var currency = TryReadString(root, "currency");
        if (!string.IsNullOrWhiteSpace(currency) && !string.Equals(currency, "try", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.SessionId), $"Stripe para birimi desteklenmiyor: {currency}")
            });
        }

        var existingSubscription = await _tenantWriteRepository.GetTenantSubscription(resolvedCompanyId, cancellationToken);
        var alreadyProcessed = existingSubscription is not null
            && existingSubscription.Status == SubscriptionStatus.Aktif
            && existingSubscription.CompanyPlanId == selectedPlan.Id
            && string.Equals(existingSubscription.PaymentProviderSubscriptionId, normalizedRequestedSessionId, StringComparison.OrdinalIgnoreCase);

        if (alreadyProcessed)
        {
            return new ConfirmStripePaymentAndActivateCommandResponse
            {
                CompanyId = resolvedCompanyId,
                PlanName = selectedPlan.PlanName,
                Status = "Aktif",
                StartDateUtc = existingSubscription!.StartDate,
                NextBillingDateUtc = existingSubscription.NextBillingDate,
                Processed = true,
                AlreadyProcessed = true,
                Message = "Bu Stripe odemesi daha once islenmis."
            };
        }

        var isPlanChanged = existingSubscription is not null && existingSubscription.CompanyPlanId != selectedPlan.Id;
        var activationResult = await _mediator.Send(
            new ActivateCompanySubscriptionCommandRequest
            {
                CompanyId = resolvedCompanyId,
                PlanSlug = expectedPlanSlug,
                PlanName = selectedPlan.PlanName,
                PaymentProviderSubscriptionId = normalizedRequestedSessionId,
                DeleteExistingCompanyData = isPlanChanged,
                ResetTenantUsage = isPlanChanged
            },
            cancellationToken);

        return new ConfirmStripePaymentAndActivateCommandResponse
        {
            CompanyId = activationResult.CompanyId,
            PlanName = activationResult.PlanName,
            Status = activationResult.Status,
            StartDateUtc = activationResult.StartDateUtc,
            NextBillingDateUtc = activationResult.NextBillingDateUtc,
            Processed = true,
            AlreadyProcessed = false,
            Message = isPlanChanged
                ? "Stripe odemesi dogrulandi. Plan degisti."
                : "Stripe odemesi dogrulandi. Abonelik aktif edildi."
        };
    }

    private string? ResolveStripeSecretKey()
    {
        return FirstNonEmpty(
            _configuration["stripe_secret_key"],
            Environment.GetEnvironmentVariable("TF_STRIPE_SECRET_KEY"),
            Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"),
            _configuration["Stripe:SecretKey"]);
    }

    private static bool TryParseClientReferenceId(string? rawClientReferenceId, out Guid companyId, out string? planSlug)
    {
        companyId = Guid.Empty;
        planSlug = null;

        if (string.IsNullOrWhiteSpace(rawClientReferenceId))
        {
            return false;
        }

        var parts = rawClientReferenceId
            .Trim()
            .Split("__", 2, StringSplitOptions.None);

        if (!Guid.TryParse(parts[0], out companyId))
        {
            return false;
        }

        if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]))
        {
            planSlug = parts[1].Trim();
        }

        return true;
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

    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement value)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        return element.TryGetProperty(propertyName, out value);
    }

    private static string? TryReadString(JsonElement element, string propertyName)
    {
        if (!TryGetProperty(element, propertyName, out var propertyValue))
        {
            return null;
        }

        return propertyValue.ValueKind == JsonValueKind.String
            ? propertyValue.GetString()
            : propertyValue.ToString();
    }

    private static long? TryReadLong(JsonElement element, string propertyName)
    {
        if (!TryGetProperty(element, propertyName, out var propertyValue))
        {
            return null;
        }

        if (propertyValue.ValueKind == JsonValueKind.Number && propertyValue.TryGetInt64(out var number))
        {
            return number;
        }

        var text = propertyValue.ValueKind == JsonValueKind.String
            ? propertyValue.GetString()
            : propertyValue.ToString();

        return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }
}

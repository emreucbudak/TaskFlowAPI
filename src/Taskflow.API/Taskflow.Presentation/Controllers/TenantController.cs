using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TenantController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("CompanyPlans")]
    public async Task<IActionResult> GetCompanyPlans()
    {
        var result = await mediator.Send(new Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesRequest());
        return Ok(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("CreateCompanyPlanCommandRequest")]
    public async Task<IActionResult> CreateCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Create.CreateCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("DeleteCompanyPlanCommandRequest")]
    public async Task<IActionResult> DeleteCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete.DeleteCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("GetAllCompanyPlanQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyPlanQueries([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("UpdateCompanyPlanCommandRequest")]
    public async Task<IActionResult> UpdateCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Update.UpdateCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("CreateStripeCheckoutSession")]
    public async Task<IActionResult> CreateStripeCheckoutSession([FromBody] CreateStripeCheckoutSessionRequest request, CancellationToken cancellationToken)
    {
        if (request is null || (string.IsNullOrWhiteSpace(request.PlanSlug) && string.IsNullOrWhiteSpace(request.PlanName)))
        {
            return BadRequest(new { message = "Plan bilgisi zorunludur." });
        }

        var stripeSecretKey =
            configuration["Stripe:SecretKey"]
            ?? configuration["stripe_secret_key"];

        if (string.IsNullOrWhiteSpace(stripeSecretKey))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Stripe secret key tanimlanmamis." });
        }

        var plans = await mediator.Send(new Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesRequest(), cancellationToken);
        var selectedPlan = plans.FirstOrDefault(plan => MatchesPlan(plan.PlanName, request.PlanSlug, request.PlanName));

        if (selectedPlan is null)
        {
            return NotFound(new { message = "Plan bulunamadi." });
        }

        if (selectedPlan.PlanPrice <= 0)
        {
            return BadRequest(new { message = "Ucretsiz plan icin Stripe odemesi olusturulamaz." });
        }

        var fallbackBaseUrl = $"{Request.Scheme}://{Request.Host}";
        var defaultSuccessUrl = configuration["Stripe:SuccessUrl"] ?? $"{fallbackBaseUrl}/checkout?status=success";
        var defaultCancelUrl = configuration["Stripe:CancelUrl"] ?? $"{fallbackBaseUrl}/checkout?status=cancel";

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
            return StatusCode((int)stripeResponse.StatusCode, new
            {
                message = "Stripe checkout session olusturulamadi.",
                detail = responseBody
            });
        }

        using var responseJson = JsonDocument.Parse(responseBody);
        var root = responseJson.RootElement;

        var checkoutUrl = root.TryGetProperty("url", out var urlElement) ? urlElement.GetString() : null;
        var sessionId = root.TryGetProperty("id", out var idElement) ? idElement.GetString() : null;

        if (string.IsNullOrWhiteSpace(checkoutUrl))
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { message = "Stripe checkout URL donmedi." });
        }

        return Ok(new
        {
            sessionId,
            checkoutUrl,
            selectedPlan.PlanName,
            selectedPlan.PlanPrice
        });
    }

    private static bool MatchesPlan(string planName, string? planSlug, string? rawPlanName)
    {
        var normalizedPlanName = NormalizePlanKey(planName);

        var slugMatch = !string.IsNullOrWhiteSpace(planSlug)
            && normalizedPlanName == NormalizePlanKey(planSlug);

        var nameMatch = !string.IsNullOrWhiteSpace(rawPlanName)
            && normalizedPlanName == NormalizePlanKey(rawPlanName);

        return slugMatch || nameMatch;
    }

    private static string NormalizePlanKey(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray();

        return new string(chars);
    }
}

public sealed class CreateStripeCheckoutSessionRequest
{
    public string? PlanSlug { get; init; }
    public string? PlanName { get; init; }
    public string? SuccessUrl { get; init; }
    public string? CancelUrl { get; init; }
}

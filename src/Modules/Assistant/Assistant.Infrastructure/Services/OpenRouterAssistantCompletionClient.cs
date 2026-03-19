using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Assistant.Application.Configuration;
using Assistant.Application.Services;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class OpenRouterAssistantCompletionClient(
    HttpClient httpClient,
    IOptions<OpenRouterOptions> openRouterOptions) : IAssistantCompletionClient
{
    public async Task<string?> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var options = openRouterOptions.Value;
        var apiKey = options.ApiKey?.Trim();
        if (string.IsNullOrWhiteSpace(apiKey) || string.Equals(apiKey, "__FROM_SECRET__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("OpenRouter ApiKey tanimli degil. Assistant chat generation calistirilamiyor.");
        }

        var models = BuildModelList(options);
        Exception? lastException = null;

        foreach (var model in models)
        {
            try
            {
                var answer = await SendCompletionAsync(
                    apiKey,
                    model,
                    systemPrompt,
                    userPrompt,
                    options,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(answer))
                {
                    return answer;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                lastException = ex;
            }
        }

        if (lastException is not null)
        {
            throw new InvalidOperationException("OpenRouter completion istegi butun modellerde basarisiz oldu.", lastException);
        }

        return null;
    }

    private async Task<string?> SendCompletionAsync(
        string apiKey,
        string model,
        string systemPrompt,
        string userPrompt,
        OpenRouterOptions options,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        if (!string.IsNullOrWhiteSpace(options.SiteUrl))
        {
            request.Headers.TryAddWithoutValidation("HTTP-Referer", options.SiteUrl.Trim());
        }

        if (!string.IsNullOrWhiteSpace(options.AppName))
        {
            request.Headers.TryAddWithoutValidation("X-Title", options.AppName.Trim());
        }

        request.Content = JsonContent.Create(
            new OpenRouterChatRequest(
                model,
                [
                    new OpenRouterMessage("system", systemPrompt),
                    new OpenRouterMessage("user", userPrompt)
                ],
                options.Temperature,
                options.MaxCompletionTokens),
            options: SerializerOptions);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"OpenRouter chat istegi basarisiz oldu. Model: {model}, StatusCode: {(int)response.StatusCode}, Response: {errorBody}");
        }

        var payload = await response.Content.ReadFromJsonAsync<OpenRouterChatResponse>(SerializerOptions, cancellationToken);
        return ExtractText(payload?.Choices);
    }

    private static string? ExtractText(IReadOnlyList<OpenRouterChoice>? choices)
    {
        var message = choices?.FirstOrDefault()?.Message;
        if (message is null)
        {
            return null;
        }

        var content = message.Content;
        if (content.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        if (content.ValueKind == JsonValueKind.String)
        {
            return content.GetString();
        }

        if (content.ValueKind == JsonValueKind.Array)
        {
            var parts = new List<string>();
            foreach (var item in content.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var rawPart = item.GetString();
                    if (!string.IsNullOrWhiteSpace(rawPart))
                    {
                        parts.Add(rawPart);
                    }
                    continue;
                }

                if (item.ValueKind == JsonValueKind.Object &&
                    item.TryGetProperty("text", out var textProperty) &&
                    textProperty.ValueKind == JsonValueKind.String)
                {
                    var text = textProperty.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        parts.Add(text);
                    }
                }
            }

            return parts.Count == 0 ? null : string.Join(Environment.NewLine, parts);
        }

        return content.ToString();
    }

    private static IReadOnlyList<string> BuildModelList(OpenRouterOptions options)
    {
        var models = new List<string>();

        void AddIfValid(string? modelId)
        {
            var trimmed = modelId?.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed) && !models.Contains(trimmed, StringComparer.Ordinal))
            {
                models.Add(trimmed);
            }
        }

        AddIfValid(options.ModelId);
        foreach (var fallbackModel in options.FallbackModels)
        {
            AddIfValid(fallbackModel);
        }

        return models;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed record OpenRouterChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<OpenRouterMessage> Messages,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_tokens")] int MaxTokens);

    private sealed record OpenRouterMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record OpenRouterChatResponse(
        [property: JsonPropertyName("choices")] IReadOnlyList<OpenRouterChoice>? Choices);

    private sealed record OpenRouterChoice(
        [property: JsonPropertyName("message")] OpenRouterChoiceMessage? Message);

    private sealed record OpenRouterChoiceMessage(
        [property: JsonPropertyName("content")] JsonElement Content);
}

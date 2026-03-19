using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Assistant.Application.Configuration;
using Assistant.Application.Services;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class GeminiEmbeddingService(
    HttpClient httpClient,
    IOptions<AssistantOptions> assistantOptions,
    IOptions<GeminiOptions> geminiOptions) : IEmbeddingService
{
    public Task<float[]> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default) =>
        CreateQueryEmbeddingAsync(text, cancellationToken);

    public Task<float[]> CreateDocumentEmbeddingAsync(string text, string? title = null, CancellationToken cancellationToken = default) =>
        CreateEmbeddingCoreAsync(text, "RETRIEVAL_DOCUMENT", title, cancellationToken);

    public Task<float[]> CreateQueryEmbeddingAsync(string text, CancellationToken cancellationToken = default) =>
        CreateEmbeddingCoreAsync(text, "RETRIEVAL_QUERY", null, cancellationToken);

    private async Task<float[]> CreateEmbeddingCoreAsync(
        string text,
        string taskType,
        string? title,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Embedding olusturmak icin metin bos olamaz.", nameof(text));
        }

        var apiKey = geminiOptions.Value.ApiKey?.Trim();
        if (string.IsNullOrWhiteSpace(apiKey) || string.Equals(apiKey, "__FROM_SECRET__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Gemini ApiKey tanimli degil. Embedding olusturulamiyor.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"models/{geminiOptions.Value.EmbeddingModelId}:embedContent");

        request.Headers.Add("x-goog-api-key", apiKey);
        request.Content = JsonContent.Create(new EmbedContentRequest(
            new EmbedContent(
                [
                    new EmbedPart(text.Trim())
                ]),
            taskType,
            title,
            assistantOptions.Value.EmbeddingDimensions),
            options: SerializerOptions);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Gemini embedding istegi basarisiz oldu. StatusCode: {(int)response.StatusCode}, Response: {errorBody}");
        }

        var payload = await response.Content.ReadFromJsonAsync<EmbedContentResponse>(cancellationToken: cancellationToken);
        var values = payload?.Embedding?.Values;
        if (values is null || values.Count == 0)
        {
            throw new InvalidOperationException("Gemini embedding yaniti bos dondu.");
        }

        return Normalize(values);
    }

    private static float[] Normalize(IReadOnlyList<float> values)
    {
        var embedding = values.ToArray();
        double sumOfSquares = 0d;
        foreach (var value in embedding)
        {
            sumOfSquares += value * value;
        }

        var magnitude = Math.Sqrt(sumOfSquares);
        if (magnitude <= 0d)
        {
            throw new InvalidOperationException("Embedding normalize edilemedi. Vektor buyuklugu sifir.");
        }

        for (var index = 0; index < embedding.Length; index++)
        {
            embedding[index] = (float)(embedding[index] / magnitude);
        }

        return embedding;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed record EmbedContentRequest(
        [property: JsonPropertyName("content")] EmbedContent Content,
        [property: JsonPropertyName("taskType")] string TaskType,
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("outputDimensionality")] int OutputDimensionality);

    private sealed record EmbedContent(
        [property: JsonPropertyName("parts")] IReadOnlyList<EmbedPart> Parts);

    private sealed record EmbedPart(
        [property: JsonPropertyName("text")] string Text);

    private sealed record EmbedContentResponse(
        [property: JsonPropertyName("embedding")] EmbedContentVector? Embedding);

    private sealed record EmbedContentVector(
        [property: JsonPropertyName("values")] IReadOnlyList<float>? Values);
}

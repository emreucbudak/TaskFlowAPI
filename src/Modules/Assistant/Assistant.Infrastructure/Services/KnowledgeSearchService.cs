using System.Security.Cryptography;
using System.Text;
using Assistant.Application.Configuration;
using Assistant.Application.Models;
using Assistant.Application.Repositories;
using Assistant.Application.Services;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class KnowledgeSearchService(
    IEmbeddingService embeddingService,
    IKnowledgeRepository knowledgeRepository,
    IKnowledgeBaseSourceReader knowledgeBaseSourceReader,
    IOptions<AssistantOptions> assistantOptions) : IKnowledgeSearchService
{
    private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
    {
        "acaba",
        "ama",
        "bir",
        "bu",
        "da",
        "de",
        "gibi",
        "hangi",
        "icin",
        "ile",
        "mi",
        "mu",
        "m\u00fc",
        "nasil",
        "ne",
        "nedir",
        "ve",
        "veya"
    };

    public async Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default)
    {
        var queryEmbedding = await embeddingService.CreateQueryEmbeddingAsync(query, cancellationToken);
        ValidateEmbeddingDimensions(queryEmbedding, assistantOptions.Value.EmbeddingDimensions);

        var take = topK.GetValueOrDefault(assistantOptions.Value.SearchTopK);
        var candidateCount = Math.Max(take * 3, 12);

        var vectorResults = await knowledgeRepository.SearchSimilarChunksAsync(queryEmbedding, candidateCount, cancellationToken);
        var lexicalResults = await SearchLexicallyAsync(query, candidateCount, cancellationToken);

        return MergeAndRank(query, vectorResults, lexicalResults, take);
    }

    private async Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchLexicallyAsync(
        string query,
        int candidateCount,
        CancellationToken cancellationToken)
    {
        var queryTokens = Tokenize(query);
        if (queryTokens.Length == 0)
        {
            return [];
        }

        var documents = await knowledgeBaseSourceReader.ReadAllAsync(cancellationToken);
        var candidates = new List<KnowledgeSearchResult>();

        foreach (var document in documents)
        {
            var documentId = CreateStableGuid($"document::{document.SourceKey}");
            var chunks = SplitLexicalChunks(document);

            for (var index = 0; index < chunks.Count; index++)
            {
                var chunk = chunks[index];
                var lexicalScore = CalculateLexicalScore(queryTokens, chunk.Title, document.SourceKey, chunk.Content);
                if (lexicalScore <= 0d)
                {
                    continue;
                }

                candidates.Add(new KnowledgeSearchResult(
                    documentId,
                    document.SourceKey,
                    chunk.Title,
                    CreateStableGuid($"chunk::{document.SourceKey}::{index}"),
                    1000 + index,
                    chunk.Content,
                    lexicalScore));
            }
        }

        return candidates
            .OrderByDescending(candidate => candidate.Score)
            .Take(candidateCount)
            .ToArray();
    }

    private static IReadOnlyCollection<KnowledgeSearchResult> MergeAndRank(
        string query,
        IReadOnlyCollection<KnowledgeSearchResult> vectorResults,
        IReadOnlyCollection<KnowledgeSearchResult> lexicalResults,
        int take)
    {
        var queryTokens = Tokenize(query);
        var merged = new Dictionary<string, KnowledgeSearchResult>(StringComparer.OrdinalIgnoreCase);

        foreach (var result in vectorResults)
        {
            merged[$"{result.SourceKey}:{result.ChunkIndex}"] = result;
        }

        foreach (var result in lexicalResults)
        {
            var key = $"{result.SourceKey}:{result.ChunkIndex}";
            if (merged.TryGetValue(key, out var existing))
            {
                merged[key] = existing with { Score = Math.Max(existing.Score, result.Score) };
                continue;
            }

            merged[key] = result;
        }

        return merged.Values
            .Select(result => new
            {
                Result = result,
                Score = CalculateHybridScore(result, queryTokens)
            })
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Result.Score)
            .Select(item => item.Result)
            .Take(take)
            .ToArray();
    }

    private static double CalculateHybridScore(KnowledgeSearchResult result, IReadOnlyCollection<string> queryTokens)
    {
        var hybridScore = result.Score;

        if (queryTokens.Count == 0)
        {
            return hybridScore;
        }

        var normalizedTitle = Normalize($"{result.Title} {result.SourceKey}");
        var normalizedChunk = Normalize(result.ChunkText);

        foreach (var token in queryTokens)
        {
            if (normalizedTitle.Contains(token, StringComparison.Ordinal))
            {
                hybridScore += 0.32d;
            }

            if (normalizedChunk.Contains(token, StringComparison.Ordinal))
            {
                hybridScore += 0.18d;
            }
        }

        if (queryTokens.Count >= 2)
        {
            foreach (var bigram in BuildBigrams(queryTokens))
            {
                if (normalizedTitle.Contains(bigram, StringComparison.Ordinal))
                {
                    hybridScore += 0.45d;
                }

                if (normalizedChunk.Contains(bigram, StringComparison.Ordinal))
                {
                    hybridScore += 0.35d;
                }
            }
        }

        return hybridScore;
    }

    private static double CalculateLexicalScore(
        IReadOnlyCollection<string> queryTokens,
        string title,
        string sourceKey,
        string chunkText)
    {
        var normalizedTitle = Normalize($"{title} {sourceKey}");
        var normalizedChunk = Normalize(chunkText);
        var tokenMatches = 0;
        var score = 0d;

        foreach (var token in queryTokens)
        {
            var matched = false;

            if (normalizedTitle.Contains(token, StringComparison.Ordinal))
            {
                score += 0.55d;
                matched = true;
            }

            if (normalizedChunk.Contains(token, StringComparison.Ordinal))
            {
                score += 0.35d;
                matched = true;
            }

            if (matched)
            {
                tokenMatches++;
            }
        }

        if (tokenMatches == 0)
        {
            return 0d;
        }

        if (tokenMatches == queryTokens.Count)
        {
            score += 0.4d;
        }

        foreach (var bigram in BuildBigrams(queryTokens))
        {
            if (normalizedTitle.Contains(bigram, StringComparison.Ordinal))
            {
                score += 0.6d;
            }

            if (normalizedChunk.Contains(bigram, StringComparison.Ordinal))
            {
                score += 0.45d;
            }
        }

        return score;
    }

    private static string[] Tokenize(string value) =>
        Normalize(value)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(token => token.Length >= 3)
            .Where(token => !StopWords.Contains(token))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

    private static string Normalize(string value)
    {
        var chars = value
            .ToLowerInvariant()
            .Select(character => character switch
            {
                '\u00e7' => 'c',
                '\u011f' => 'g',
                '\u0131' => 'i',
                '\u00f6' => 'o',
                '\u015f' => 's',
                '\u00fc' => 'u',
                _ => character
            })
            .Select(character => char.IsLetterOrDigit(character) ? character : ' ')
            .ToArray();

        return string.Join(
            ' ',
            new string(chars)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static IReadOnlyList<LexicalChunk> SplitLexicalChunks(KnowledgeBaseFile document)
    {
        var lines = document.Content
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.None);

        var chunks = new List<LexicalChunk>();
        var currentSectionTitle = document.Title;
        var buffer = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                FlushChunk(chunks, document.Title, currentSectionTitle, buffer);
                currentSectionTitle = line[3..].Trim();
                continue;
            }

            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                currentSectionTitle = line[2..].Trim();
                continue;
            }

            buffer.Add(line);
        }

        FlushChunk(chunks, document.Title, currentSectionTitle, buffer);

        if (chunks.Count == 0)
        {
            chunks.Add(new LexicalChunk(document.Title, document.Content));
        }

        return chunks;
    }

    private static void FlushChunk(
        ICollection<LexicalChunk> chunks,
        string documentTitle,
        string sectionTitle,
        ICollection<string> buffer)
    {
        if (buffer.Count == 0)
        {
            return;
        }

        var title = string.Equals(documentTitle, sectionTitle, StringComparison.OrdinalIgnoreCase)
            ? documentTitle
            : $"{documentTitle} - {sectionTitle}";

        chunks.Add(new LexicalChunk(title, string.Join(' ', buffer)));
        buffer.Clear();
    }

    private static IEnumerable<string> BuildBigrams(IReadOnlyCollection<string> tokens)
    {
        var tokenArray = tokens.ToArray();
        for (var index = 0; index < tokenArray.Length - 1; index++)
        {
            yield return $"{tokenArray[index]} {tokenArray[index + 1]}";
        }
    }

    private static Guid CreateStableGuid(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var bytes = hash[..16];
        return new Guid(bytes);
    }

    private sealed record LexicalChunk(string Title, string Content);

    private static void ValidateEmbeddingDimensions(float[] embedding, int expectedDimensions)
    {
        if (embedding.Length != expectedDimensions)
        {
            throw new InvalidOperationException(
                $"Embedding boyutu uyusmuyor. Beklenen: {expectedDimensions}, gelen: {embedding.Length}.");
        }
    }
}

using Assistant.Application.Services;

namespace Assistant.Infrastructure.Services;

public sealed class MarkdownTextChunker : ITextChunker
{
    private static readonly char[] SentenceBoundaries = ['.', '!', '?', '\n'];

    public IReadOnlyList<string> Split(string text, int targetSize, int overlapSize)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
        if (normalized.Length <= targetSize)
        {
            return [normalized];
        }

        var chunks = new List<string>();
        var start = 0;

        while (start < normalized.Length)
        {
            var remainingLength = normalized.Length - start;
            var windowLength = Math.Min(targetSize, remainingLength);
            var window = normalized.Substring(start, windowLength);
            var end = start + windowLength;

            if (end < normalized.Length)
            {
                var paragraphBreak = window.LastIndexOf("\n\n", StringComparison.Ordinal);
                if (paragraphBreak >= targetSize / 2)
                {
                    end = start + paragraphBreak;
                }
                else
                {
                    var sentenceBreak = window.LastIndexOfAny(SentenceBoundaries);
                    if (sentenceBreak >= targetSize / 2)
                    {
                        end = start + sentenceBreak + 1;
                    }
                }
            }

            if (end <= start)
            {
                end = start + windowLength;
            }

            var chunk = normalized[start..end].Trim();
            if (!string.IsNullOrWhiteSpace(chunk))
            {
                chunks.Add(chunk);
            }

            if (end >= normalized.Length)
            {
                break;
            }

            start = Math.Max(end - overlapSize, start + 1);
        }

        return chunks;
    }
}

using System.Security.Cryptography;
using System.Text;
using Assistant.Application.Configuration;
using Assistant.Application.Models;
using Assistant.Application.Services;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class KnowledgeBaseFileReader(IOptions<AssistantOptions> assistantOptions) : IKnowledgeBaseSourceReader
{
    public async Task<IReadOnlyCollection<KnowledgeBaseFile>> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        var knowledgeBasePath = ResolveKnowledgeBasePath(assistantOptions.Value.KnowledgeBasePath);
        if (!Directory.Exists(knowledgeBasePath))
        {
            return [];
        }

        var files = Directory.EnumerateFiles(knowledgeBasePath, "*.md", SearchOption.AllDirectories)
            .OrderBy(filePath => Path.GetRelativePath(knowledgeBasePath, filePath), StringComparer.OrdinalIgnoreCase)
            .ToList();

        var documents = new List<KnowledgeBaseFile>(files.Count);

        foreach (var filePath in files)
        {
            var rawContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            var content = Normalize(rawContent);
            var title = ResolveTitle(filePath, content);
            var sourceKey = Path.GetRelativePath(knowledgeBasePath, filePath).Replace('\\', '/');
            var checksum = ComputeChecksum(content);

            documents.Add(new KnowledgeBaseFile(sourceKey, title, content, checksum));
        }

        return documents;
    }

    private static string ResolveKnowledgeBasePath(string configuredPath) =>
        Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath));

    private static string Normalize(string content) =>
        content.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();

    private static string ResolveTitle(string filePath, string content)
    {
        var titleLine = content.Split('\n')
            .FirstOrDefault(line => line.StartsWith("# ", StringComparison.Ordinal));

        if (!string.IsNullOrWhiteSpace(titleLine))
        {
            return titleLine[2..].Trim();
        }

        return Path.GetFileNameWithoutExtension(filePath);
    }

    private static string ComputeChecksum(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}

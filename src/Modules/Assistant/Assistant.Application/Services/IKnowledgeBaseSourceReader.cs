using Assistant.Application.Models;

namespace Assistant.Application.Services;

public interface IKnowledgeBaseSourceReader
{
    Task<IReadOnlyCollection<KnowledgeBaseFile>> ReadAllAsync(CancellationToken cancellationToken = default);
}

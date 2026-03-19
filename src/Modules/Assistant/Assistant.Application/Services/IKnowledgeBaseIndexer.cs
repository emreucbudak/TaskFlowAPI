using Assistant.Application.Models;

namespace Assistant.Application.Services;

public interface IKnowledgeBaseIndexer
{
    Task<KnowledgeIndexResult> IndexAsync(CancellationToken cancellationToken = default);
}

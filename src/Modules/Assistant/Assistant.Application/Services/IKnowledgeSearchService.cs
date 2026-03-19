using Assistant.Application.Models;

namespace Assistant.Application.Services;

public interface IKnowledgeSearchService
{
    Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default);
}

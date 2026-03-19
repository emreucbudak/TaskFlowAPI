using Assistant.Application.Services;
using Microsoft.Extensions.Logging;

namespace Assistant.Infrastructure.Services;

public sealed class AssistantInitializationService(
    IKnowledgeBaseIndexer knowledgeBaseIndexer,
    ILogger<AssistantInitializationService> logger) : IAssistantInitializationService
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var result = await knowledgeBaseIndexer.IndexAsync(cancellationToken);
        logger.LogInformation(
            "Assistant knowledge base senkronizasyonu tamamlandi. FilesProcessed: {FilesProcessed}, FilesSkipped: {FilesSkipped}, ChunksIndexed: {ChunksIndexed}",
            result.FilesProcessed,
            result.FilesSkipped,
            result.ChunksIndexed);
    }
}

using Assistant.Application.Configuration;
using Assistant.Application.Services;
using Assistant.Infrastructure.Services;
using Assistant.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assistant.Infrastructure.Extensions;

public static class AssistantInfrastructureServiceExtensions
{
    public static IServiceCollection AddAssistantInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AssistantOptions>()
            .Bind(configuration.GetSection("Assistant"))
            .Validate(
                options => options.EmbeddingDimensions == AssistantOptions.DefaultEmbeddingDimensions,
                $"Assistant:EmbeddingDimensions degeri {AssistantOptions.DefaultEmbeddingDimensions} olmali. Farkli bir boyuta gecilecekse Assistant migration'i da guncellenmeli.")
            .ValidateOnStart();

        services.AddAssistantPersistence(configuration);
        services.AddScoped<IKnowledgeBaseSourceReader, KnowledgeBaseFileReader>();
        services.AddSingleton<ITextChunker, MarkdownTextChunker>();
        services.AddScoped<IKnowledgeBaseIndexer, KnowledgeBaseIndexer>();
        services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();

        return services;
    }
}

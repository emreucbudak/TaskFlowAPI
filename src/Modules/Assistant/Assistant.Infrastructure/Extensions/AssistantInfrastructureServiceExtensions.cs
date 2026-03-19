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
            .Validate(
                options => options.MinimumSourceScore >= 0d && options.MinimumSourceScore <= 1d,
                "Assistant:MinimumSourceScore 0 ile 1 arasinda olmali.")
            .ValidateOnStart();

        services.AddOptions<GeminiOptions>()
            .Bind(configuration.GetSection("Gemini"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.EmbeddingModelId), "Gemini:EmbeddingModelId bos olamaz.")
            .ValidateOnStart();

        services.AddAssistantPersistence(configuration);
        services.AddHttpClient<IEmbeddingService, GeminiEmbeddingService>(client =>
        {
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddScoped<IKnowledgeBaseSourceReader, KnowledgeBaseFileReader>();
        services.AddSingleton<ITextChunker, MarkdownTextChunker>();
        services.AddScoped<IKnowledgeBaseIndexer, KnowledgeBaseIndexer>();
        services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddScoped<IAssistantInitializationService, AssistantInitializationService>();
        services.AddScoped<IAssistantChatService, AssistantChatService>();

        return services;
    }
}

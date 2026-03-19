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

        services.AddOptions<OpenRouterOptions>()
            .Bind(configuration.GetSection("OpenRouter"))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ModelId), "OpenRouter:ModelId bos olamaz.")
            .Validate(options => options.Temperature >= 0d && options.Temperature <= 2d, "OpenRouter:Temperature 0 ile 2 arasinda olmali.")
            .Validate(options => options.MaxCompletionTokens > 0, "OpenRouter:MaxCompletionTokens sifirdan buyuk olmali.")
            .ValidateOnStart();

        services.AddAssistantPersistence(configuration);
        services.AddHttpClient<IEmbeddingService, GeminiEmbeddingService>(client =>
        {
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient<OpenRouterAssistantCompletionClient>(client =>
        {
            client.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        services.AddScoped<GeminiAssistantCompletionClient>();
        services.AddScoped<IAssistantCompletionClient, AssistantCompletionClient>();
        services.AddScoped<IKnowledgeBaseSourceReader, KnowledgeBaseFileReader>();
        services.AddSingleton<ITextChunker, MarkdownTextChunker>();
        services.AddScoped<IKnowledgeBaseIndexer, KnowledgeBaseIndexer>();
        services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddScoped<IAssistantInitializationService, AssistantInitializationService>();
        services.AddScoped<IAssistantChatService, AssistantChatService>();

        return services;
    }
}

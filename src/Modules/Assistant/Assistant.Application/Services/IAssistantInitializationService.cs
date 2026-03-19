namespace Assistant.Application.Services;

public interface IAssistantInitializationService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

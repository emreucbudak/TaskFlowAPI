namespace Chat.Application.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
namespace Identity.Application.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}

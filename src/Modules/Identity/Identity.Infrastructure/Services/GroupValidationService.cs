using Identity.Application.Repositories;
using Identity.Application.Services;

namespace Identity.Infrastructure.Services
{
    public class GroupValidationService : IGroupValidationService
    {
        private readonly IReadRepository<Domain.Entities.Groups,Guid> _repository;

        public GroupValidationService(IReadRepository<Domain.Entities.Groups, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            var groups = await _repository.GetByIdAsync(false,userId);
            if (groups == null)
            {
                return false;
            }
            return true;
        }
    }
}
using Identity.Application.Repositories;
using Identity.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services
{
    public class GroupValidationService : IGroupValidationService
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _repository;

        public GroupValidationService(IReadRepository<Domain.Entities.Groups, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            var group = await _repository.GetByIdAsync(
                false,
                groupId,
                query => query.Include(item => item.Users));

            return group?.Users.Any(member => member.UserId == userId) == true;
        }
    }
}

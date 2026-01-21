using FlashMediator;

namespace TaskFlow.BuildingBlocks.Contracts.UserGroups
{
    public class GetUserAllGroupsNameQueriesRequest : IRequest<List<string>>
    {
        public Guid userId { get; set; }
        public GetUserAllGroupsNameQueriesRequest(Guid userId)
        {
            this.userId = userId;
        }
    }
}

using FlashMediator;

namespace TaskFlow.BuildingBlocks.Contracts.UserGroups
{
    public class GetUserAllGroupsNameQueriesRequest : IRequest<List<string>>
    {
        public Guid userId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public GetUserAllGroupsNameQueriesRequest(Guid userId, int pageSize, int pageNumber)
        {
            this.userId = userId;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}

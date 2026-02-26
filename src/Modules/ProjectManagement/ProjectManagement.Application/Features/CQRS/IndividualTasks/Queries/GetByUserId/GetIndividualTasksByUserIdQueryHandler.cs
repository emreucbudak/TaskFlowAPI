using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId
{
    public class GetIndividualTasksByUserIdQueryHandler : IRequestHandler<GetIndividualTasksByUserIdQueryRequest, PagedResult<GetIndividualTasksByUserIdQueryResponse>>
    {
        private readonly IProjectManagementReadRepository _readRepository;

        public GetIndividualTasksByUserIdQueryHandler(IProjectManagementReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PagedResult<GetIndividualTasksByUserIdQueryResponse>> Handle(GetIndividualTasksByUserIdQueryRequest request, CancellationToken cancellationToken)
        {
            var tasks = await _readRepository.GetAllIndividualTasks(false, request.PageNumber, request.PageSize,request.UserId);

            return new PagedResult<GetIndividualTasksByUserIdQueryResponse>
            {
                Items = tasks.Select(task => new GetIndividualTasksByUserIdQueryResponse(
                    task.Id,
                    task.AssignedUserId,
                    task.TaskTitle,
                    task.Description,
                    task.Deadline,
                    "Açık",
                    "Bireysel",
                    string.IsNullOrWhiteSpace(task.TaskPriority?.CategoryName)
                        ? "Belirtilmedi"
                        : task.TaskPriority.CategoryName)).ToList(),
                TotalCount = tasks.Count(),
                Page = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

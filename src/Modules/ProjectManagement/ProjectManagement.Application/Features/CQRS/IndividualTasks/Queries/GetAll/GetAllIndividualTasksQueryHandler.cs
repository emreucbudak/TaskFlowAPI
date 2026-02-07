using FlashMediator;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetAll
{
    public class GetAllIndividualTasksQueryHandler : IRequestHandler<GetAllIndividualTasksQueryRequest, List<GetAllIndividualTasksQueryResponse>>
    {
        private readonly IProjectManagementReadRepository _readRepository;

        public GetAllIndividualTasksQueryHandler(IProjectManagementReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<GetAllIndividualTasksQueryResponse>> Handle(GetAllIndividualTasksQueryRequest request, CancellationToken cancellationToken)
        {
            var tasks = await _readRepository.GetAllIndividualTasks(false, request.PageNumber, request.PageSize);
            return tasks.Select(task => new GetAllIndividualTasksQueryResponse(task.Id, task.AssignedUserId, task.TaskTitle, task.Description, task.Deadline)).ToList();
        }
    }
}

using FlashMediator;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId
{
    public class GetIndividualTasksByUserIdQueryHandler : IRequestHandler<GetIndividualTasksByUserIdQueryRequest, List<GetIndividualTasksByUserIdQueryResponse>>
    {
        private readonly IProjectManagementReadRepository _readRepository;

        public GetIndividualTasksByUserIdQueryHandler(IProjectManagementReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<GetIndividualTasksByUserIdQueryResponse>> Handle(GetIndividualTasksByUserIdQueryRequest request, CancellationToken cancellationToken)
        {
            var tasks = await _readRepository.GetIndividualTasksByUserId(request.UserId, false);

            return tasks.Select(task => new GetIndividualTasksByUserIdQueryResponse(task.Id, task.AssignedUserId, task.TaskTitle, task.Description, task.Deadline)).ToList();
        }
    }
}

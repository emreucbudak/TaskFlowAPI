using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById
{
    public class GetIndividualTaskByIdQueryHandler : IRequestHandler<GetIndividualTaskByIdQueryRequest, GetIndividualTaskByIdQueryResponse>
    {
        private readonly IProjectManagementReadRepository _readRepository;

        public GetIndividualTaskByIdQueryHandler(IProjectManagementReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<GetIndividualTaskByIdQueryResponse> Handle(GetIndividualTaskByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var task = await _readRepository.GetIndividualTask(request.Id, false);
            if (task == null)
            {
                throw new IndividualTaskNotFoundException(request.Id);
            }

            return new GetIndividualTaskByIdQueryResponse(task.Id, task.AssignedUserId, task.TaskTitle, task.Description, task.Deadline);
        }
    }
}

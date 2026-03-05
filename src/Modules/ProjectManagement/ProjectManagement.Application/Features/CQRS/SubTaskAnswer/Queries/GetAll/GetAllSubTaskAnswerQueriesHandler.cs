using FlashMediator;
using ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Exceptions;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll
{
    public class GetAllSubTaskAnswerQueriesHandler : IRequestHandler<GetAllSubTaskAnswerQueriesRequest, List<GetAllSubTaskAnswerQueriesResponse>>
    {
        private readonly IProjectManagementReadRepository _repository;

        public GetAllSubTaskAnswerQueriesHandler(IProjectManagementReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAllSubTaskAnswerQueriesResponse>> Handle(GetAllSubTaskAnswerQueriesRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.TaskId, false, cancellationToken);
            if (task is null)
            {
                throw new SubTaskAnswerNotFoundExceptions();
            }
            var subTaskAnswers = task.GetAllSubTaskAnswer(request.SubTaskId);
            var Answers = subTaskAnswers.Select(x=> new GetAllSubTaskAnswerQueriesResponse()
            {
                AnswerText = x.AnswerText,
                SenderId = x.SenderId,
            }).ToList();
            return Answers;

        }
    }
}


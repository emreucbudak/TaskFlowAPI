using FlashMediator;
using ProjectManagement.Application.Repositories;

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
            var task = await _repository.GetTask(request.TaskId, false);
            if (task is null)
            {
                throw new Exception("task bulunamadı");
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

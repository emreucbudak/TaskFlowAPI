using FlashMediator;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Queries.GetAll
{
    public class GetAllTaskAnswerQueriesHandler : IRequestHandler<GetAllTaskAnswerQueriesRequest, List<GetAllTaskAnswerQueriesResponse>>
    {
        private readonly IProjectManagementReadRepository _repository;

        public GetAllTaskAnswerQueriesHandler(IProjectManagementReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAllTaskAnswerQueriesResponse>> Handle(GetAllTaskAnswerQueriesRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.Id, false);
            var taskAnswer = task.GetAllTaskAnwers();
            return taskAnswer.Select(x => new GetAllTaskAnswerQueriesResponse
            {
                AnswerText = x.AnswerText,
                SenderId = x.SenderId,
            }).ToList();
        }
    }
}

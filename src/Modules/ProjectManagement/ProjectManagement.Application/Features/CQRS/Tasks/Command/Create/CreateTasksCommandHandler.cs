using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Create
{
    public class CreateTasksCommandHandler : IRequestHandler<CreateTasksCommandRequest>
    {
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public CreateTasksCommandHandler(
            ICapUnitOfWork unitOfWork,
            IProjectManagementWriteRepository writeRepository,
            ICapPublisher capPublisher)
        {
            _unitOfWork = unitOfWork;
            _writeRepository = writeRepository;
            _capPublisher = capPublisher;
        }

        public async Task Handle(CreateTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var deadlineDate = DateOnly.FromDateTime(request.DeadlineTime);
            if (deadlineDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Bitiş tarihi şuandan önce (geçmiş tarih olamaz)");
            }

            var task = new Domain.Entities.Task(
                request.TaskName,
                request.Description,
                deadlineDate,
                DateOnly.FromDateTime(DateTime.UtcNow));
            var delayTime = deadlineDate
                .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
                .AddDays(-1) - DateTime.UtcNow;

            if (delayTime < TimeSpan.Zero)
            {
                delayTime = TimeSpan.Zero;
            }

            await using var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false);
            try
            {
                await _writeRepository.AddTask(task);

                await _capPublisher.PublishDelayAsync(
                    delayTime,
                    "TaskCreated",
                    new TaskCreatedIntegrationEvent(
                        task.Id,
                        task.TaskName,
                        task.Description,
                        task.DeadlineTime),
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

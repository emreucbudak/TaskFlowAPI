using DotNetCore.CAP;
using FlashMediator;
using Report.Application.IntegrationEvents;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Report.Application.Features.CQRS.Reports.Command.Create
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommandRequest>
    {
        private readonly IReportWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _reportProducer;

        public CreateReportCommandHandler(IReportWriteRepository writeRepository, ICapUnitOfWork unitOfWork, ICapPublisher reportProducer)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _reportProducer = reportProducer;
        }

        public async Task Handle(CreateReportCommandRequest request, CancellationToken cancellationToken)
        {
            var report = new Domain.Entities.Report(
                request.ReportTopicId,
                request.Description,
                request.UserId,
                request.ReportStatusId,
                request.Title,
                request.NotifiedDepartmentId
            );

            await using var transaction = _unitOfWork.BeginTransaction(_reportProducer, autoCommit: false);
            try
            {
                await _writeRepository.AddAsync(report);
                await _reportProducer.PublishAsync(
                    "report.created",
                    new ReportCreatedIntegrationEvent(report.Id, report.Description, report.NotifiedDepartmantId),
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

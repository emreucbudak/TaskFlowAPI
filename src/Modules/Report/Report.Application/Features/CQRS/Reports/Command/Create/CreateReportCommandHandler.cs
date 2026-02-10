using FlashMediator;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Report.Application.Features.CQRS.Reports.Command.Create
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommandRequest>
    {
        private readonly IReportWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReportCommandHandler(IReportWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateReportCommandRequest request, CancellationToken cancellationToken)
        {
            var report = new Domain.Entities.Report(
                request.ReportTopicId, 
                request.Description, 
                request.UserId, 
                request.ReportStatusId, 
                request.Title, 
                request.NotifiedDepartmantId
            );

            await _writeRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

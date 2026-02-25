using FlashMediator;
using Report.Application.Features.CQRS.Reports.Exceptions;
using Report.Application.Repositories;
using Report.Application.UnitOfWork;

namespace Report.Application.Features.CQRS.Reports.Command.Update
{
    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommandRequest>
    {
        private readonly IReportReadRepository _readRepository;
        private readonly IReportWriteRepository _writeRepository;
        private readonly IReportUnitOfWork _unitOfWork;

        public UpdateReportCommandHandler(IReportReadRepository readRepository, IReportWriteRepository writeRepository, IReportUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateReportCommandRequest request, CancellationToken cancellationToken)
        {
            var report = await _readRepository.GetByIdAsync(true, request.Id);
            if (report is null)
            {
                throw new ReportNotFoundException();
            }

            if (request.ReportTopicId.HasValue)
                report.UpdateReportTopic(request.ReportTopicId.Value);
            
            if (!string.IsNullOrWhiteSpace(request.Description))
                report.UpdateDescription(request.Description);

            if (request.ReportStatusId.HasValue)
                report.UpdateReportStatus(request.ReportStatusId.Value);

            if (!string.IsNullOrWhiteSpace(request.Title))
                report.UpdateTitle(request.Title);

            _writeRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

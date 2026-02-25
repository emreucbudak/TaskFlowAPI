using FlashMediator;
using Report.Application.Features.CQRS.Reports.Exceptions;
using Report.Application.Repositories;
using Report.Application.UnitOfWork;

namespace Report.Application.Features.CQRS.Reports.Command.Delete
{
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommandRequest>
    {
        private readonly IReportReadRepository _readRepository;
        private readonly IReportWriteRepository _writeRepository;
        private readonly IReportUnitOfWork _unitOfWork;

        public DeleteReportCommandHandler(IReportReadRepository readRepository, IReportWriteRepository writeRepository, IReportUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteReportCommandRequest request, CancellationToken cancellationToken)
        {
            var report = await _readRepository.GetByIdAsync(false, request.Id);
            if (report == null)
            {
                throw new ReportNotFoundException();
            }

            _writeRepository.DeleteAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

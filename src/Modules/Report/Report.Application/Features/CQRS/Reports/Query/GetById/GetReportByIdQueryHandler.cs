using FlashMediator;
using Report.Application.Features.CQRS.Reports.Exceptions;
using Report.Application.Repositories;

namespace Report.Application.Features.CQRS.Reports.Query.GetById
{
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQueryRequest, GetReportByIdQueryResponse>
    {
        private readonly IReportReadRepository _readRepository;

        public GetReportByIdQueryHandler(IReportReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<GetReportByIdQueryResponse> Handle(GetReportByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var report = await _readRepository.GetByIdAsync(false, request.Id);
            if (report == null)
            {
                throw new ReportNotFoundException();
            }

            return new GetReportByIdQueryResponse(
                report.Id,
                report.ReportTopicId,
                report.Title,
                report.Description,
                report.ReportingUserId,
                report.ReportStatusId,
                report.NotifiedDepartmantId,
                report.CreatedAt
            );
        }
    }
}

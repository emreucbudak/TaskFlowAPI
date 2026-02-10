using TaskFlow.BuildingBlocks.Exceptions;

namespace Report.Application.Features.CQRS.Reports.Exceptions
{
    public class ReportNotFoundException : NotFoundExceptions
    {
        public ReportNotFoundException() : base("Rapor bulunamadı.")
        {
        }
    }
}

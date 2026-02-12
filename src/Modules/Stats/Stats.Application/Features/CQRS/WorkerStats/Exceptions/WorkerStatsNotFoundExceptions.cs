using TaskFlow.BuildingBlocks.Exceptions;

namespace Stats.Application.Features.CQRS.WorkerStats.Exceptions
{
    public class WorkerStatsNotFoundExceptions : NotFoundExceptions
    {
        public WorkerStatsNotFoundExceptions(DateOnly period) : base($"{period} dönemi için kullanıcıya ait istatistik bulunamadı!")
        {
        }
    }
}

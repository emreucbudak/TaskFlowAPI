using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Company.Exceptions
{
    public class CompanyNotFoundExceptions : NotFoundExceptions
    {
        public CompanyNotFoundExceptions() : base("Şirket bulunamadı!")
        {
        }
    }
}

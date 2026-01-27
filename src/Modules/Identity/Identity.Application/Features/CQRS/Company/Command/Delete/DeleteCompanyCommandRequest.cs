using FlashMediator;

namespace Identity.Application.Features.CQRS.Company.Command.Delete
{
    public record DeleteCompanyCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public DeleteCompanyCommandRequest(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}

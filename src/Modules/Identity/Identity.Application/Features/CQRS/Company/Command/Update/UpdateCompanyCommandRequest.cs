using FlashMediator;

namespace Identity.Application.Features.CQRS.Company.Command.Update
{
    public record UpdateCompanyCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public string CompanyName { get; init; }
        public UpdateCompanyCommandRequest(Guid id, string companyName)
        {
            Id = id;
            CompanyName = companyName;
        }
    }
}

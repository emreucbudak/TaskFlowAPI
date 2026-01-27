using FlashMediator;

namespace Identity.Application.Features.CQRS.Company.Command.Create
{
    public record CreateCompanyCommandRequest : IRequest
    {
        public string CompanyName { get; init; }

        public CreateCompanyCommandRequest(string companyName)
        {
            CompanyName = companyName;
        }
    }
}

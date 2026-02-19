using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;



namespace Identity.Application.Features.CQRS.Company.Command.Create
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommandRequest, Guid>
    {
        private readonly IWriteRepository<Domain.Entities.Company> _companyWriteRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(IWriteRepository<Domain.Entities.Company> companyWriteRepository, IIdentityCapUnitOfWork unitOfWork)
        {
            _companyWriteRepository = companyWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = new Domain.Entities.Company(request.CompanyName);
            await _companyWriteRepository.AddAsync(company);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return company.Id;
        }
    }
}

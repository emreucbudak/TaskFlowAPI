using FlashMediator;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;



namespace Identity.Application.Features.CQRS.Company.Command.Create
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommandRequest>
    {
        private readonly IWriteRepository<Domain.Entities.Company> _companyWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(IWriteRepository<Domain.Entities.Company> companyWriteRepository, IUnitOfWork unitOfWork)
        {
            _companyWriteRepository = companyWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = new Domain.Entities.Company(request.CompanyName);
            await _companyWriteRepository.AddAsync(company);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

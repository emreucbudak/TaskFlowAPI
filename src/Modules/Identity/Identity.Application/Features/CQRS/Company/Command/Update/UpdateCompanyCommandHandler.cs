using FlashMediator;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Company.Command.Update
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommandRequest>
    {
        private readonly IWriteRepository<Domain.Entities.Company> _companyWriteRepository;
        private readonly IReadRepository<Domain.Entities.Company, Guid> _companyReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(IWriteRepository<Domain.Entities.Company> companyWriteRepository, IReadRepository<Domain.Entities.Company, Guid> companyReadRepository, IUnitOfWork unitOfWork)
        {
            _companyWriteRepository = companyWriteRepository;
            _companyReadRepository = companyReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = await _companyReadRepository.GetByIdAsync(true,request.Id);
            company.UpdateCompanyName(request.CompanyName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}

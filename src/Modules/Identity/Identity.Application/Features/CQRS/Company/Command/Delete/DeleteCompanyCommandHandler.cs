using FlashMediator;
using Identity.Application.Features.CQRS.Company.Exceptions;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Company.Command.Delete
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Company, Guid> _companyReadRepository;
        private readonly IWriteRepository<Domain.Entities.Company> _companyWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCompanyCommandHandler(IReadRepository<Domain.Entities.Company, Guid> companyReadRepository, IWriteRepository<Domain.Entities.Company> companyWriteRepository)
        {
            _companyReadRepository = companyReadRepository;
            _companyWriteRepository = companyWriteRepository;
        }

        public async Task Handle(DeleteCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            var company = await _companyReadRepository.GetByIdAsync(false,request.CompanyId);
            if (company is null)
            {
                throw new CompanyNotFoundExceptions();
            }
            await _companyWriteRepository.DeleteAsync(company);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}

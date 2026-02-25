using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Department.Command.Add
{
    public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommandRequest>
    {
        private readonly IWriteRepository<Identity.Domain.Entities.Department> _writeRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public AddDepartmentCommandHandler(
            IWriteRepository<Domain.Entities.Department> writeRepository,
            IIdentityCapUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(AddDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = new Domain.Entities.Department(request.Name, request.companyId);
            await _writeRepository.AddAsync(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"getallcompanydepartments:{request.companyId}");
        }
    }
}

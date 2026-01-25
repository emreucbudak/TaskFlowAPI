using FlashMediator;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Department.Command.Add
{
    public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommandRequest>
    {
        private readonly IWriteRepository<Identity.Domain.Entities.Department> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddDepartmentCommandHandler(IWriteRepository<Domain.Entities.Department> writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = new Domain.Entities.Department(request.Name, request.companyId);
            await _writeRepository.AddAsync(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}

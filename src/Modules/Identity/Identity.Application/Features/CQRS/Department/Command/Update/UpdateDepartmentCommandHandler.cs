using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Department.Command.Update
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommandRequest>
    {
        private readonly IReadRepository<Identity.Domain.Entities.Department, Guid> _readRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateDepartmentCommandHandler(IReadRepository<Domain.Entities.Department, Guid> readRepository, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = await _readRepository.GetByIdAsync(true,request.Id);
            if (department is null)
            {
                throw new DepartmentNotFoundExceptions();
            }
            department.UpdateName(request.Name);
            await unitOfWork.SaveChangesAsync(cancellationToken);


        }
    }
}

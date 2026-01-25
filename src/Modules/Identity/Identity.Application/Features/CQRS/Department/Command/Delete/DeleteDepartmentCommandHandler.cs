using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.Repositories;
using System.Net;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Department.Command.Delete
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommandRequest>
    {
        private readonly IWriteRepository<Identity.Domain.Entities.Department> _writeRepository;
        private readonly IReadRepository<Identity.Domain.Entities.Department,Guid> _readRepository;
        private readonly IUnitOfWork _unitOfWork;


        public DeleteDepartmentCommandHandler(IWriteRepository<Domain.Entities.Department> writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = await _readRepository.GetByIdAsync(false,request.DepartmentId);
            if (department is null)
            {
                throw new DepartmentNotFoundExceptions();
            }
            await _writeRepository.DeleteAsync(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


        }
    }
}

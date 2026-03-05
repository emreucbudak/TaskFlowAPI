using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using System.Net;

namespace Identity.Application.Features.CQRS.Department.Command.Delete
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommandRequest>
    {
        private readonly IWriteRepository<Identity.Domain.Entities.Department> _writeRepository;
        private readonly IReadRepository<Identity.Domain.Entities.Department,Guid> _readRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;


        public DeleteDepartmentCommandHandler(
            IWriteRepository<Domain.Entities.Department> writeRepository,
            IReadRepository<Domain.Entities.Department, Guid> readRepository,
            IIdentityCapUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = await _readRepository.GetByIdAsync(false,request.DepartmentId);
            if (department is null)
            {
                throw new DepartmentNotFoundExceptions();
            }
             _writeRepository.Delete(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


        }
    }
}


using FlashMediator;
using Identity.Application.Repositories;

namespace Identity.Application.Features.CQRS.Department.Command.Update
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommandRequest>
    {
        private readonly IReadRepository<Identity.Domain.Entities.Department, Guid> _readRepository;
        private readonly IWriteRepository<Identity.Domain.Entities.Department> _writeRepository;

        public UpdateDepartmentCommandHandler(IReadRepository<Domain.Entities.Department, Guid> readRepository, IWriteRepository<Domain.Entities.Department> writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        public async Task Handle(UpdateDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var department = await _readRepository.GetByIdAsync(false,request.Id);
            department.UpdateName(request.Name);


        }
    }
}

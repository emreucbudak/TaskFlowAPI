using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Department.Exceptions
{
    public class DepartmentNotFoundExceptions : NotFoundExceptions
    {
        public DepartmentNotFoundExceptions(Guid id) : base($"{id}'e sahip departman bulunamadı!")
        {
        }
    }
}

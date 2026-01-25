using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Department.Exceptions
{
    public class DepartmentNotFoundExceptions : NotFoundExceptions
    {
        public DepartmentNotFoundExceptions() : base($"Departman bulunamadı!")
        {
        }
    }
}

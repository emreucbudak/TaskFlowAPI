using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

    }
}

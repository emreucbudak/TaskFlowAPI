using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
    }
}

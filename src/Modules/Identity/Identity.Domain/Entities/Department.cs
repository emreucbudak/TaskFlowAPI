namespace Identity.Domain.Entities
{
    public class Department
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<User> Users { get; set; }

    }
}

using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities
{
    public class Department : BaseEntity
    {
        public Department(string name, Guid companyId)
        {
            Name = name;
            CompanyId = companyId;
        }

        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Department name cannot be empty.");
            }
            Name = newName;
        }
        public void AddUser(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            _users.Add(user);
        }


    }
}

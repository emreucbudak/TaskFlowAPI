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
                throw new ArgumentException("Departman ismi boş veya null olamaz.");
            }
            Name = newName;
        }
        public void AddUser(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "Çalışan null olamaz.");
            }
            if(_users.Any(u => u.Id == user.Id))
            {
                throw new InvalidOperationException("Çalışan zaten gruba üye.");
            }
            _users.Add(user);
        }


    }
}

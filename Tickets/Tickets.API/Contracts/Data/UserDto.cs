using RepairShop.Infrastructure;

namespace Tickets.Contracts.Data
{
    public class UserDto : IEntity
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }

        public UserDto(Guid id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}

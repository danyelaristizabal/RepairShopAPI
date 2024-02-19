using RepairShop.Infrastructure;

namespace Users.Contracts.Data
{
    public class UserDto : IEntity
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string ContactInformation { get; init; }

        public UserDto(Guid id, string name, string email, string contactInformation)
        {
            Id = id;
            Name = name;
            Email = email;
            ContactInformation = contactInformation;
        }
    }
}

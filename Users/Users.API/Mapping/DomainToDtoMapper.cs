using Users.Contracts.Data;
using Users.Domain;

namespace Users.Mapping
{
    public static class DomainToDtoMapper
    {
        public static User ToUser(this UserDto user)
        {
            return new User(user.Id, user.Name, user.Email, user.ContactInformation);
        }
        public static UserDto ToUserDto(this User user)
        {
            return new UserDto(user.Id, user.Name, user.Email, user.ContactInformation);
        }
    }
}

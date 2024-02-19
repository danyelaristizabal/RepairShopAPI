using Users.Contracts.Responses;
using Users.Domain;

namespace Users.Mapping;

public static class DomainToApiContractMapper
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(user.Id, user.Name, user.Email, user.ContactInformation);
    }

    public static GetAllUsersResponse ToCustomersResponse(this IEnumerable<User> users)
    {
        return new GetAllUsersResponse
        {
            Users = users.Select(x => new UserResponse(x.Id, x.Name, x.Email, x.ContactInformation))
        };
    }
}

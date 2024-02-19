
using Users.Contracts.Requests;
using Users.Domain;

namespace Users.Mapping;

public static class ApiContractToDomainMapper
{
    public static User ToUser(this UserRequest request)
    {
        return new User(Guid.NewGuid(), request.Name, request.Email, request.ContactInformation);
    }

    public static User ToUser(this UpdateUserRequest request)
    {
        return new User(request.Id, request.User.Name, request.User.Email, request.User.ContactInformation);
    }
}

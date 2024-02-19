using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Users.Contracts.Requests
{
    public class UpdateUserRequest
    {
        [FromRoute(Name = "id"), Required] public Guid Id { get; init; }

        [FromBody] public UserRequest User { get; set; } = default!;
    }
}

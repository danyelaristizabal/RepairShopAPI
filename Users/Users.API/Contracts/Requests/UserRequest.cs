using System.ComponentModel.DataAnnotations;

namespace Users.Contracts.Requests
{
    public record UserRequest([Required, MaxLength(50)] string Name, [Required, EmailAddress] string Email, [MaxLength(200)] string ContactInformation);
}

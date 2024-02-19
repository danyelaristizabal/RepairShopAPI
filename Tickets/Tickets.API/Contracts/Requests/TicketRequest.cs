using System.ComponentModel.DataAnnotations;

namespace Tickets.Contracts.Requests
{
    public record TicketRequest([Required] Guid UserId, [Required, MaxLength(500)] string Description);

}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tickets.Contracts.Requests
{
    public class UpdateTicketRequest
    {
        [FromRoute(Name = "id"), Required] public Guid Id { get; init; }
        [FromBody, Required] public TicketRequest Ticket { get; set; } = default!;
    }
}


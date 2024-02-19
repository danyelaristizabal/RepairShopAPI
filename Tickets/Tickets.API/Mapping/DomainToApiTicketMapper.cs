using Tickets.Contracts.Responses;
using Tickets.Domain.Models;

public static class DomainToApiTicketMapper
{
    public static TicketResponse ToTicketResponse(this Ticket ticket)
    {
        return new TicketResponse(ticket.Id, ticket.UserName, ticket.Description, ticket.DateOfIssue);
    }

    public static GetAllTicketsResponse ToTicketsResponse(this IEnumerable<Ticket> tickets)
    {
        return new GetAllTicketsResponse
        {
            Tickets = tickets.Select(x => new TicketResponse(x.Id, x.UserName, x.Description, x.DateOfIssue))
        };
    }
}

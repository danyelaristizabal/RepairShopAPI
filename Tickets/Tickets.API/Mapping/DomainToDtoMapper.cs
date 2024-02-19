using Tickets.Contracts.Data;
using Tickets.Domain.Models;

public static class DomainToDtoMapper
{
    public static Ticket ToTicket(this TicketDto ticket, string? userName)
    {
        return new Ticket(ticket.Id, userName, ticket.Description, ticket.DateOfIssue);
    }
    public static TicketDto ToTicketDto(this Ticket ticket, Guid userId)
    {
        return new TicketDto(ticket.Id, userId, ticket.Description, ticket.DateOfIssue);
    }
}

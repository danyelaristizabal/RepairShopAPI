using Tickets.Contracts.Requests;
using Tickets.Domain.Models;

public static class ApiContractToDomainMapper
{
    public static Ticket ToTicket(this TicketRequest request)
    {
        return new Ticket(Guid.NewGuid(), null, request.Description, DateTime.Now);
    }

    public static Ticket ToTicket(this UpdateTicketRequest request, DateTime dateOfIssue)
    {
        return new Ticket(request.Id, null, request.Ticket.Description, dateOfIssue);
    }
}

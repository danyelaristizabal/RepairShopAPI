namespace Tickets.Contracts.Responses
{
    public record TicketResponse(Guid Id, string? UserName, string Description, DateTime DateOfIssue);
}

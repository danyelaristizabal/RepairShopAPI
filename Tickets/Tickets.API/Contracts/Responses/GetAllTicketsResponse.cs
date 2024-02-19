namespace Tickets.Contracts.Responses
{
    public class GetAllTicketsResponse
    {
        public IEnumerable<TicketResponse> Tickets { get; init; } = Enumerable.Empty<TicketResponse>();
    }
}

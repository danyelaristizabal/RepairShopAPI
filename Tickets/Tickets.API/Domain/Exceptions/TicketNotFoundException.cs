public class TicketNotFoundException : Exception
{
    public TicketNotFoundException() : base("Ticket not found in the repository.")
    {
    }

    public TicketNotFoundException(string message) : base(message)
    {
    }

    public TicketNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}

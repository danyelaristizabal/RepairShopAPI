public class InvalidTicketException : Exception
{
    public InvalidTicketException() : base("Invalid ticket.")
    {
    }

    public InvalidTicketException(string message) : base(message)
    {
    }

    public InvalidTicketException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class InvalidUserIdException : Exception
{
    public InvalidUserIdException() : base("Invalid user ID.")
    {
    }

    public InvalidUserIdException(string message) : base(message)
    {
    }

    public InvalidUserIdException(string message, Exception inner) : base(message, inner)
    {
    }
}

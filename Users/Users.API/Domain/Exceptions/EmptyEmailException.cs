public class EmptyEmailException : Exception
{
    public EmptyEmailException()
        : base("Email cannot be empty.")
    {
    }
}

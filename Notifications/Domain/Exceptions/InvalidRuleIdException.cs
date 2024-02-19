namespace Notifications.Domain.Exceptions
{
    public class InvalidRuleIdException : Exception
    {
        public InvalidRuleIdException()
            : base("Rule ID cannot be empty.")
        {
        }
    }
}

namespace Notifications.Domain.Exceptions
{
    public class UserIdRequiredException : Exception
    {
        public UserIdRequiredException()
            : base("UserId cannot be null when RuleType is Individual.")
        {
        }
    }
}

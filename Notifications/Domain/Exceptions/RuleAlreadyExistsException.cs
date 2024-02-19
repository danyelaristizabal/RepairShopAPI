namespace Notifications.Domain.Exceptions
{
    public class RuleAlreadyExistsException : Exception
    {
        public RuleAlreadyExistsException(Guid id)
            : base($"A rule with the ID {id} already exists.")
        {
        }
    }
}

namespace Notifications.Contracts.Responses
{
    public class GetAllRulesResponse
    {
        public IEnumerable<RuleResponse> Rules { get; init; } = Enumerable.Empty<RuleResponse>();
    }
}

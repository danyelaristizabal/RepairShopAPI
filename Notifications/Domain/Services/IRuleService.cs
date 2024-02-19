namespace Notifications.Domain.Services
{
    public interface IRuleService
    {
        Task<Rule?> GetAsync(Guid ruleId);
        Task<IEnumerable<Rule>> GetAllAsync();
        Task<bool> CreateAsync(Rule rule);
        Task<bool> UpdateAsync(Rule rule);
        Task<bool> DeleteAsync(Guid id);

        Task ReloadCacheAsync();
    }
}

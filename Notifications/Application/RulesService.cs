using Notifications.Contracts.Data;
using Notifications.Domain;
using Notifications.Domain.Enums;
using Notifications.Domain.Exceptions;
using Notifications.Domain.Services;
using Notifications.Mapping;
using RepairShop.Infrastructure;

namespace Notifications.Application.Services
{
    public class RulesService : IRuleService
    {
        private readonly IRepository<RuleDto> _ruleRepository;
        private readonly ICacheRepository<RuleDto> _rulesCachedRepository;

        public RulesService(IRepository<RuleDto> ruleRepository, ICacheRepository<RuleDto> rulesCachedRepository) // Updated constructor
        {
            _ruleRepository = ruleRepository;
            _rulesCachedRepository = rulesCachedRepository;
        }

        public async Task<Rule?> GetAsync(Guid ruleId)
        {
            if (ruleId == Guid.Empty) throw new InvalidRuleIdException();


            var rule = await _ruleRepository.GetAsync(rule => rule.Id == ruleId);
            return rule.ToRule();
        }

        public async Task<IEnumerable<Rule>> GetAllAsync()
        {
            var ruleEntities = await _ruleRepository.GetAllListAsync();
            return ruleEntities.Select(ruleDto => ruleDto.ToRule());
        }

        public async Task<bool> CreateAsync(Rule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (rule.RuleType == RuleType.Individual && rule.UserId == null)
            {
                throw new UserIdRequiredException();
            }

            var existingRule = await _ruleRepository.GetAsync(item => item.Id == rule.Id);
            if (existingRule != null) throw new RuleAlreadyExistsException(rule.Id);

            var ruleInRepo = await _ruleRepository.GetAsync(item => item.Id == rule.Id);

            if (ruleInRepo == null)
            {
                await _ruleRepository.CreateAsync(rule.ToRuleDto());
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(Rule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (rule.RuleType == RuleType.Individual && rule.UserId == null)
            {
                throw new UserIdRequiredException();
            }

            var ruleInRepo = await _ruleRepository.GetAsync(item => item.Id == rule.Id);

            if (ruleInRepo != null)
            {
                await _ruleRepository.UpdateAsync(rule.ToRuleDto());
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) throw new InvalidRuleIdException();

            var ruleInRepo = await _ruleRepository.GetAsync(item => item.Id == id);

            if (ruleInRepo != null)
            {
                await _ruleRepository.RemoveAsync(id);
                return true;
            }
            return false;
        }
        public async Task ReloadCacheAsync() // New method
        {
            await _rulesCachedRepository.ReloadCacheAsync();
        }
    }
}

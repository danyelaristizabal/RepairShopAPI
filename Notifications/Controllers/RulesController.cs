using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notifications.Contracts.Requests;
using Notifications.Domain.Exceptions;
using Notifications.Domain.Services;
using Notifications.Mapping;
using RepairShop.Common.Helpers;

namespace Notifications.Controllers
{
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly IRuleService _ruleService;
        private readonly ILogger<RulesController> _logger;

        public RulesController(IRuleService ruleService, ILogger<RulesController> logger)
        {
            _ruleService = ruleService;
            _logger = logger;
        }

        [HttpPost("rules")]
        public async Task<IActionResult> Create([FromBody] RuleRequest request)
        {
            try
            {
                var rule = request.ToRule();

                if (await _ruleService.CreateAsync(rule))
                {
                    var ruleResponse = rule.ToRuleResponse();

                    return CreatedAtAction("Get", new { ruleResponse.Id }, ruleResponse);
                }

                return StatusCode(500, "An error occurred while creating the rule.");
            }
            catch (RuleAlreadyExistsException)
            {
                return Conflict("A rule with this ID already exists.");
            }
            catch (UserIdRequiredException)
            {
                return BadRequest("UserId cannot be null when RuleType is Individual.");
            }
            catch (InvalidRuleIdException)
            {
                return BadRequest("Rule ID cannot be empty.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the rule.");
            }
        }

        [HttpGet("rules/{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {

            try
            {
                var rule = await _ruleService.GetAsync(id);

                if (rule is null)
                {
                    return NotFound();
                }

                var ruleResponse = rule.ToRuleResponse();
                return Ok(ruleResponse);
            }
            catch (InvalidRuleIdException)
            {
                return BadRequest("Rule ID cannot be empty.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("rules")]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ruleService.GetAllAsync();
            return Ok(rules);
        }

        [HttpPut("rules/{id:guid}")]
        public async Task<IActionResult> Update([FromMultiSource] UpdateRuleRequest request)
        {
            try
            {
                var existingRule = await _ruleService.GetAsync(request.Id);

                if (existingRule is null)
                {
                    return NotFound();
                }

                var rule = request.ToRule();
                if (await _ruleService.UpdateAsync(rule))
                {
                    var ruleResponse = rule.ToRuleResponse();
                    return Ok(ruleResponse);
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the rule.");
                }
            }
            catch (UserIdRequiredException)
            {
                return BadRequest("UserId cannot be null when RuleType is Individual.");
            }
            catch (InvalidRuleIdException)
            {
                return BadRequest("Rule ID cannot be empty.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("rules/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var deleted = await _ruleService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (InvalidRuleIdException)
            {
                return BadRequest("Rule ID cannot be empty.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpPost("rules/reload-cache")]
        public async Task<IActionResult> ReloadCache()
        {
            await _ruleService.ReloadCacheAsync();
            return Ok("Cache reloaded successfully.");
        }
    }
}

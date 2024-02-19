using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Notifications.Contracts.Requests
{
    public class UpdateRuleRequest
    {
        [FromRoute(Name = "id"), Required] public Guid Id { get; init; }

        [FromBody] public RuleRequest Rule { get; set; } = default!;
    }
}

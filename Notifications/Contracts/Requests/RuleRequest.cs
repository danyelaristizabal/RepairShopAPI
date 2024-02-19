using Notifications.Domain.Enums;
using RepairShop.Common.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notifications.Contracts.Requests
{
    public record RuleRequest
    {
        [Required, MinLength(1)]
        [JsonConverter(typeof(JsonStringEnumEnumerableConverter<NotificationType>))]
        public IEnumerable<NotificationType> NotificationTypes { get; init; }

        [Required, Range(0, int.MaxValue)]
        public int AllowedTimeIntervalInMinutes { get; init; }

        [Required, Range(0, int.MaxValue)]
        public int Rate { get; init; }

        [Required, EnumDataType(typeof(RuleType))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RuleType RuleType { get; init; }
        [JsonConverter(typeof(NullableGuidConverter))]
        public Guid? UserId { get; init; }
    }
}

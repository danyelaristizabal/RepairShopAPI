using RepairShop.Infrastructure;

namespace Notifications.Contracts.Data
{
    public class UserNotificationsHistoryDto : IEntity
    {
        private Guid _userId;
        public Guid Id
        {
            get => _userId;
            init => _userId = value;
        }
        public Dictionary<string, List<DateTimeOffset>> HistoryOfNotificationsSent { get; set; } = new();
        public UserNotificationsHistoryDto(Guid userId)
        {
            _userId = userId;
        }
    }
}

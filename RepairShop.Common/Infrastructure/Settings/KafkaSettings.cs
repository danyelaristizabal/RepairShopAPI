namespace RepairShop.Infrastructure.Settings
{
    public class KafkaSettings
    {
        public string? Domain { get; init; }
        public string? Port { get; init; }
        public string[]? TopicsToCreate { get; init; }
        public string Host => $"{Domain}:{Port}";

    }
}

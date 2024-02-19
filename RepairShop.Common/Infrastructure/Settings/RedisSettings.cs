namespace RepairShop.Common.Infrastructure.Settings
{
    public class RedisSettings
    {
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? InstanceName { get; set; }
        public string ConnectionString => $"{Host}:{Port}";

    }
}

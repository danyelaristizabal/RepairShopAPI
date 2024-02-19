namespace RepairShop.Common.Infrastructure.Settings
{
    public class MessagebusSettings
    {
        public string? AssemblyName { get; init; }
        public Dictionary<string, string>? Consumers { get; init; }
        public Dictionary<string, string>? Publishers { get; init; }
        public string? Group { get; set; }

    }
}

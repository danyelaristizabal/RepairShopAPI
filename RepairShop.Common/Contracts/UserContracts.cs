namespace RepairShop.Common.Contracts
{
    public record UserUpdated(Guid ItemId, string Name, string Email);
    public record UserCreated(Guid ItemId, string Name, string Email);
    public record UserDeleted(Guid ItemId);
}

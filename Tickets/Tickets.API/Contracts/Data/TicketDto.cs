using RepairShop.Infrastructure;

namespace Tickets.Contracts.Data
{
    public class TicketDto : IEntity
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Description { get; init; }
        public DateTime DateOfIssue { get; set; }
        public TicketDto(Guid id, Guid userId, string description, DateTime dateOfIssue)
        {
            Id = id;
            UserId = userId;
            Description = description;
            DateOfIssue = dateOfIssue;
        }
    }
}

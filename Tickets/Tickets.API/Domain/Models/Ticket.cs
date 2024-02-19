namespace Tickets.Domain.Models
{
    public class Ticket
    {
        public Guid Id { get; init; }
        public string? UserName { get; set; }
        public string Description { get; init; }
        public DateTime DateOfIssue { get; init; }

        public Ticket(Guid id, string? userName, string description, DateTime dateOfIssue)
        {
            Id = id;
            UserName = userName;
            Description = description;
            DateOfIssue = dateOfIssue;
        }
    }
}

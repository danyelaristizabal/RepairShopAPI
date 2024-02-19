using Tickets.Domain.Models;

namespace Tickets.Domain.Services
{
    public interface ITicketsService
    {
        Task<bool> CreateAsync(Ticket ticket, Guid userId);

        Task<Ticket?> GetAsync(Guid id);

        Task<IEnumerable<Ticket>> GetAllAsync();

        Task<bool> UpdateAsync(Ticket ticket, Guid userId);

        Task<bool> DeleteAsync(Guid id);
    }
}

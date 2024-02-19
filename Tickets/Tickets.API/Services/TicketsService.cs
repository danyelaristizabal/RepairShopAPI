using MassTransit;
using RepairShop.Common.Contracts;
using RepairShop.Common.Domain.Enums;
using RepairShop.Infrastructure;
using Tickets.Contracts.Data;
using Tickets.Domain.Models;
using Tickets.Domain.Services;

namespace Tickets.Services
{
    public class TicketsService : ITicketsService
    {
        private readonly IRepository<TicketDto> _ticketsRepository;
        private readonly IRepository<UserDto> _usersRepository;
        private readonly ITopicProducer<NotificationContract> _notificationPublisher;

        public TicketsService(
            IRepository<TicketDto> ticketsRepository,
            IRepository<UserDto> usersRepository,
            ITopicProducer<NotificationContract> notificationPublisher)
        {
            _ticketsRepository = ticketsRepository;
            _usersRepository = usersRepository;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<Ticket?> GetAsync(Guid id)
        {
            if (id == Guid.Empty) throw new InvalidTicketException();

            var ticketDto = await _ticketsRepository.GetAsync(ticket => ticket.Id == id);

            if (ticketDto is null)
                return null;

            var userInRepo = await _usersRepository.GetAsync(user => user.Id == ticketDto.UserId);

            if (userInRepo == null)
            {
                throw new UserNotFoundException();
            }

            return ticketDto.ToTicket(userInRepo.Name);
        }
        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            var ticketDtos = await _ticketsRepository.GetAllListAsync();
            var usersDtos = await _usersRepository.GetAllListAsync();

            return ticketDtos.Select(ticketDto => ticketDto.ToTicket(usersDtos.FirstOrDefault(userDto => userDto.Id == ticketDto.UserId)?.Name));
        }


        public async Task<bool> CreateAsync(Ticket ticket, Guid userId)
        {
            if (ticket == null) throw new InvalidTicketException();
            if (userId == Guid.Empty) throw new InvalidUserIdException();

            var userInRepo = await _usersRepository.GetAsync(item => item.Id == userId);

            if (userInRepo == null)
            {
                throw new UserNotFoundException();
            }

            var ticketInRepo = await _ticketsRepository.GetAsync(item => item.Id == ticket.Id);

            if (ticketInRepo is null)
            {
                var dto = ticket.ToTicketDto(userInRepo.Id);
                await _ticketsRepository.CreateAsync(dto);
                ticket.UserName = userInRepo.Name;
                await _notificationPublisher.Produce(
                    new NotificationContract(
                        Guid.NewGuid(),
                        userInRepo.Id,
                        DateTimeOffset.Now,
                        NotificationType.TicketCreated,
                        $"Hello {userInRepo.Name} ! your ticket with Id {ticket.Id} was just created. " +
                        $"Thank you for choosing us as your repairshop of choice.", //Hardcoded: This should be stored on persistant storage 
                        NotificationSystemType.Email,
                        userInRepo.Email));
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(Ticket ticket, Guid userId)
        {
            if (ticket == null) throw new InvalidTicketException();
            if (userId == Guid.Empty) throw new InvalidUserIdException();

            var ticketInRepo = await _ticketsRepository.GetAsync(item => item.Id == ticket.Id);
            var userInRepo = await _usersRepository.GetAsync(user => user.Id == userId);

            if (userInRepo == null)
            {
                throw new UserNotFoundException();
            }

            if (ticketInRepo is not null)
            {
                await _ticketsRepository.UpdateAsync(new TicketDto(ticketInRepo.Id, userInRepo.Id, ticket.Description, ticket.DateOfIssue));
                ticket.UserName = userInRepo.Name;
                await _notificationPublisher.Produce(
                new NotificationContract(
                    Guid.NewGuid(),
                    userInRepo.Id,
                    DateTimeOffset.Now,
                    NotificationType.TicketUpdated,
                    $"Hello {userInRepo.Name}! your ticket with Id {ticket.Id} was just updated. " +
                    $"Thank you for choosing us as your repairshop of choice.", //Hardcoded: This should be stored on persistant storage 
                    NotificationSystemType.Email,
                    userInRepo.Email));
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Ticket ID cannot be empty.");
            var ticketInRepo = await _ticketsRepository.GetAsync(item => item.Id == id);
            var userInRepo = await _usersRepository.GetAsync(user => user.Id == ticketInRepo.UserId);


            if (ticketInRepo != null)
            {
                await _ticketsRepository.RemoveAsync(id);
                await _notificationPublisher.Produce(
              new NotificationContract(
                  Guid.NewGuid(),
                  userInRepo.Id,
                  DateTimeOffset.Now,
                  NotificationType.TicketDeleted,
                  $"Hello {userInRepo?.Name}! your ticket with Id {ticketInRepo.Id} was just Deleted. " +
                  $"Thank you for choosing us as your repairshop of choice.", //Hardcoded: This should be stored on persistant storage 
                  NotificationSystemType.Email,
                  userInRepo?.Email));
                return true;
            }

            return false;
        }
    }
}

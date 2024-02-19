using MassTransit;
using RepairShop.Common.Contracts;
using RepairShop.Common.Domain.Enums;
using RepairShop.Infrastructure;
using Users.Contracts.Data;
using Users.Domain;
using Users.Domain.Exceptions;
using Users.Domain.Services;
using Users.Mapping;

namespace Users.Application.Services
{
    public class UsersService : IUserService
    {
        private readonly IRepository<UserDto> _userRepository;
        private readonly ITopicProducer<UserCreated> _userCreatedPublisher;
        private readonly ITopicProducer<UserUpdated> _userUpdaterPublisher;
        private readonly ITopicProducer<UserDeleted> _userDeletedPublisher;
        private readonly ITopicProducer<NotificationContract> _notificationPublisher;


        public UsersService(IRepository<UserDto> userRepository,
            ITopicProducer<UserCreated> userCreatedPublisher,
            ITopicProducer<UserUpdated> userUpdaterPublisher,
            ITopicProducer<UserDeleted> userDeletedPublisher,
            ITopicProducer<NotificationContract> notificationPublisher)
        {
            _userRepository = userRepository;
            _userCreatedPublisher = userCreatedPublisher;
            _userUpdaterPublisher = userUpdaterPublisher;
            _userDeletedPublisher = userDeletedPublisher;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<User?> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            var user = await _userRepository.GetAsync(user => user.Id == userId);
            return user.ToUser();
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var userEntities = await _userRepository.GetAllListAsync();
            return userEntities.Select(userDto => userDto.ToUser());
        }
        public async Task<bool> CreateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new EmptyEmailException();

            var existingUser = await _userRepository.GetAsync(item => item.Email == user.Email);
            if (existingUser != null) throw new UserAlreadyExistsException(user.Email);

            var userInRepo = await _userRepository.GetAsync(item => item.Id == user.Id);

            if (userInRepo == null)
            {
                await _userRepository.CreateAsync(user.ToUserDto());
                await _userCreatedPublisher.Produce(new UserCreated(user.Id, user.Name, user.Email));
                await _notificationPublisher.Produce(
                    new NotificationContract(
                        Guid.NewGuid(),
                        user.Id,
                        DateTimeOffset.Now,
                        NotificationType.UserCreated,
                        $"Thank you, {user.Name} for choosing as your repairshop of choice", // Hardcoded: should be stored on persistant storage 
                        NotificationSystemType.Email,
                        user.Email));
                return true;
            }

            return false;
        }
        public async Task<bool> UpdateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new EmptyEmailException();

            var userInRepo = await _userRepository.GetAsync(item => item.Id == user.Id);

            if (userInRepo != null)
            {
                await _userRepository.UpdateAsync(new UserDto(userInRepo.Id, user.Name, user.Email, user.ContactInformation));
                await _userUpdaterPublisher.Produce(new UserUpdated(user.Id, user.Name, user.Email));
                await _notificationPublisher.Produce(
                   new NotificationContract(
                       Guid.NewGuid(),
                       user.Id,
                       DateTimeOffset.Now,
                       NotificationType.UserUpdated,
                       $"Dear {user.Name} your user info was succesfully updated. ", // Hardcoded: should be stored on persistant storage 
                       NotificationSystemType.Email,
                       user.Email));
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(id));

            var userInRepo = await _userRepository.GetAsync(item => item.Id == id);

            if (userInRepo != null)
            {
                await _userRepository.RemoveAsync(id);
                await _userDeletedPublisher.Produce(new UserDeleted(userInRepo.Id));
                await _notificationPublisher.Produce(
                   new NotificationContract(
                       Guid.NewGuid(),
                       userInRepo.Id,
                       DateTimeOffset.Now,
                       NotificationType.UserDeleted,
                       $"Dear {userInRepo.Name} we are sorry to see you let you go.", // Hardcoded: should be stored on persistant storage 
                       NotificationSystemType.Email,
                       userInRepo.Email));
                return true;
            }
            return false;
        }
    }
}


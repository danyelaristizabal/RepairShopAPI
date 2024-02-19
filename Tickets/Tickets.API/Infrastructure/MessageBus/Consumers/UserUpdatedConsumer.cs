using MassTransit;
using RepairShop.Common.Contracts;
using RepairShop.Infrastructure;
using Tickets.Contracts.Data;

namespace Tickets.Infrastructure.MessageBus.Consumers
{
    public class UserUpdatedConsumer : IConsumer<UserUpdated>
    {
        private readonly IRepository<UserDto> _userRepository;

        public UserUpdatedConsumer(IRepository<UserDto> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            var message = context.Message;
            var itemInDB = await _userRepository.GetAsync(message.ItemId);
            var item = new UserDto(message.ItemId, message.Name, message.Email);
            if (itemInDB == null)
            {
                await _userRepository.CreateAsync(item);
                return;
            }
            await _userRepository.UpdateAsync(item);
        }
    }
}

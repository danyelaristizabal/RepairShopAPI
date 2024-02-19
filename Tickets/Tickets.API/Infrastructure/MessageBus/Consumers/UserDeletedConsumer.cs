using MassTransit;
using RepairShop.Common.Contracts;
using RepairShop.Infrastructure;
using Tickets.Contracts.Data;

namespace Tickets.Infrastructure.MessageBus.Consumers
{
    public class UserDeletedConsumer : IConsumer<UserDeleted>
    {
        private readonly IRepository<UserDto> _userRepository;

        public UserDeletedConsumer(IRepository<UserDto> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task Consume(ConsumeContext<UserDeleted> context)
        {
            var message = context.Message;
            var item = await _userRepository.GetAsync(message.ItemId);
            if (item == null) return;
            await _userRepository.RemoveAsync(message.ItemId);
        }
    }
}

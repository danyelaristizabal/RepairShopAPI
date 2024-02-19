using MassTransit;
using RepairShop.Common.Contracts;
using RepairShop.Infrastructure;
using Tickets.Contracts.Data;

namespace Tickets.Infrastructure.MessaegBus.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly IRepository<UserDto> _userRepository;

        public UserCreatedConsumer(IRepository<UserDto> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            var message = context.Message;
            var item = await _userRepository.GetAsync(message.ItemId);
            if (item != null) return;
            item = new UserDto(message.ItemId, message.Name, message.Email);
            await _userRepository.CreateAsync(item);
        }
    }
}

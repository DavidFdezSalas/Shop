using MassTransit;
using Shop.Shared.Events;

namespace Shop.Notifications
{
    internal class UserRegisteredConsumer : IConsumer<UserCreatedEvent>
    {
        private ILogger<UserRegisteredConsumer> _logger;

        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;

            _logger.LogInformation("User created event received for userId: {UserId}, email: {Email}", user.userId, user.email);

            return Task.CompletedTask;
        }
    }
}

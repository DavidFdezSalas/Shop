using MassTransit;
using Shop.Shared.Events;

namespace Shop.Notifications
{
    internal class UserRegisteredConsumer : IConsumer<UserCreatedEvent>
    {
        private ILogger<UserRegisteredConsumer> _logger;
        private IEmailService _emailService;

        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;

            _logger.LogInformation("User created event received for userId: {UserId}, email: {Email}", user.userId, user.email);

            _emailService.SendWelcomeMail(user.email);

            return Task.CompletedTask;
        }
    }
}

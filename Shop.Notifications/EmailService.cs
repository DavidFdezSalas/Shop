using MailKit.Net.Smtp;

namespace Shop.Notifications
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendWelcomeMail(string toEmail)
        {
            using var client = new SmtpClient();

            var host = _configuration.GetSection("Email:SmtpHost").Value;
            var port = int.Parse(_configuration.GetSection("Email:SmtpPort").Value!);
            client.Connect(host, port, false);

            var fromEmail = _configuration.GetSection("Email:FromAddress").Value;
            var fromName = _configuration.GetSection("Email:FromName").Value;
            var subject = "Welcome to Shop!";
            var body = "Thank you for registering at Shop. We're excited to have you on board!";
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress(fromName!, fromEmail!));
            message.To.Add(new MimeKit.MailboxAddress(toEmail, toEmail));
            message.Subject = subject;
            message.Body = new MimeKit.TextPart("plain")
            {
                Text = body
            };
            await client.SendAsync(message);
            _logger.LogInformation($"Welcome email sent to {toEmail}");

        }

    }
}

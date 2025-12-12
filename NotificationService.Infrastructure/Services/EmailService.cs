using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Sending email to {Recipient} with subject {Subject}", to, subject);

            if (!MailboxAddress.TryParse(to, out var mailbox))
            {
                _logger.LogWarning("Invalid email address: {Recipient}", to);
                return false;
            }

            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_configuration["MailSettings:Email"]);
                email.From.Add(new MailboxAddress(
                    _configuration["MailSettings:DisplayName"],
                    _configuration["MailSettings:Email"]
                ));

                email.To.Add(mailbox); // use validated mailbox
                email.Subject = subject;

                var builder = new BodyBuilder { TextBody = body };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    _configuration["MailSettings:Host"],
                    int.Parse(_configuration["MailSettings:Port"]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["MailSettings:Email"],
                    _configuration["MailSettings:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", to);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient} with subject {Subject}", to, subject);
                return false;
            }
        }

    }
}

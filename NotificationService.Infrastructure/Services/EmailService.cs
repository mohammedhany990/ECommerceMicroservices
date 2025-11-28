using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();

                email.Sender = MailboxAddress.Parse(_configuration["MailSettings:Email"]);

                email.From.Add(new MailboxAddress(
                    _configuration["MailSettings:DisplayName"],
                    _configuration["MailSettings:Email"]
                ));


                email.To.Add(MailboxAddress.Parse(to));

                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    TextBody = body
                };

                email.Body = builder.ToMessageBody();

                // SMTP
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

                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }
    }
}
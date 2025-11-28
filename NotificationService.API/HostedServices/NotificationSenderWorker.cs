using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.API.HostedServices
{
    public class NotificationSenderWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationSenderWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IRepository<Notification>>();
                var email = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var pending = await repository.GetUnsentAsync();

                foreach (var notif in pending)
                {
                    var sent = await email.SendAsync(notif.To, notif.Subject, notif.Body);

                    if (sent)
                    {
                        notif.IsSent = true;
                        notif.SentAt = DateTime.UtcNow;
                        await repository.UpdateAsync(notif);
                        await repository.SaveChangesAsync();
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

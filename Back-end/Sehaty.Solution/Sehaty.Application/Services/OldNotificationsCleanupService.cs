using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services
{
    public class OldNotificationsCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OldNotificationsCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanOldNotifications();


                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
        private async Task CleanOldNotifications()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SehatyDbContext>();

            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var oldNotifications = await db.Notifications
                .Where(n => n.CreatedAt <= cutoffDate)
                .ToListAsync();

            if (oldNotifications.Any())
            {
                db.Notifications.RemoveRange(oldNotifications);
                await db.SaveChangesAsync();
            }
        }
    }
}

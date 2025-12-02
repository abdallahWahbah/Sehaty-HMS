using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services
{
    public class CancelUnpaidAppointmentsService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public CancelUnpaidAppointmentsService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("from");
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SehatyDbContext>();

                    var limitTime = DateTime.Now.AddHours(-2);

                    var unpaidAppointments = context.Appointments
                        .Where(a => a.Status == AppointmentStatus.Pending &&
                                    a.BookingDateTime <= limitTime)
                        .ToList();

                    foreach (var a in unpaidAppointments)
                    {
                        a.Status = AppointmentStatus.Canceled;
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sehaty.Core.Specifications.Appointment_Specs;

namespace Sehaty.Application.Services
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppointmentReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    var smsSender = scope.ServiceProvider.GetRequiredService<ISmsSender>();
                    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                    var spec = new AppointmentSpecifications();
                    var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
                    var targetTime = DateTime.UtcNow.AddHours(24);
                    var filteredAppointments = appointments
                        .Where(a =>
                               a.Status != AppointmentStatus.Canceled &&
                                Math.Abs((a.AppointmentDateTime - targetTime).TotalMinutes) <= 1
                        ).ToList();

                    foreach (var item in filteredAppointments)
                    {
                        string message = $"تذكير: لديك موعد مع الطبيب {item.Doctor.FirstName} {item.Doctor.LastName} بتاريخ {item.AppointmentDateTime}";

                        var notificationDto = new CreateNotificationDto
                        {
                            UserId = item.PatientId,
                            Title = "Appointment Reminder",
                            Message = message,
                            Priority = NotificationPriority.High,
                            RelatedEntityType = "Appointment",
                            RelatedEntityId = item.Id,
                            SentViaEmail = false,
                            SentViaSMS = false,
                            NotificationType = NotificationType.Appointment,
                            IsRead = false
                        };

                        var notification = mapper.Map<Notification>(notificationDto);
                        await unit.Repository<Notification>().AddAsync(notification);
                        await unit.CommitAsync();

                        if (!string.IsNullOrEmpty(item.Patient.User.Email))
                        {
                            var filepath = $"{env.WebRootPath}/templates/ConfirmationReminder.html";
                            var body = File.ReadAllText(filepath);

                            body = body.Replace("[header]", message)
                                .Replace("[url]", $"https://localhost:7086/api/Appointments/ConfirmAppointment/{item.Id}")
                                .Replace("[linkTitle]", "تأكيد الموعد")
                                .Replace("[body]", "هذا تذكير بأن موعدكم الطبي قريب. يُرجى تأكيد الحضور أو طلب تعديل الموعد إذا لزم الأمر.")
                                .Replace("[imageUrl]", "https://res.cloudinary.com/dl21kzp79/image/upload/v1763917652/icon-positive-vote-1_1_dpzjrw.png");

                            await emailSender.SendEmailAsync(item.Patient.User.Email, "Sehaty - Appointment Reminder", body);
                            notificationDto.SentViaEmail = true;
                        }

                        //if (!string.IsNullOrEmpty(item.Patient.User.PhoneNumber))
                        //{
                        //    smsSender.SendSmsAsync(item.Patient.User.PhoneNumber, message);
                        //    notificationDto.SentViaSMS = true;
                        //}
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}

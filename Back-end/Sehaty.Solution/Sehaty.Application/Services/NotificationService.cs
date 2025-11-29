namespace Sehaty.Application.Services
{
    public class NotificationService(IMapper mapper, IUnitOfWork unit, IWebHostEnvironment env, IEmailSender emailSender) : INotificationService
    {
        public async Task<bool> CreateNotificationForAppointmentConfirmation(Appointment appointment)
        {
            var patient = await unit.Repository<Patient>().GetByIdAsync(appointment.PatientId);

            if (patient == null) return false;

            string message = $"تم تأكيد موعدك مع الطبيب {appointment.Doctor.FirstName} {appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime}";

            var notificationDto = new CreateNotificationDto
            {
                UserId = appointment.PatientId,
                Title = "Appointment Confirmed",
                Message = message,
                Priority = NotificationPriority.High,
                RelatedEntityType = "Appointment",
                RelatedEntityId = appointment.Id,
                SentViaEmail = false,
                SentViaSMS = true,
                NotificationType = NotificationType.Appointment,
                IsRead = false
            };
            var notification = mapper.Map<Notification>(notificationDto);
            await unit.Repository<Notification>().AddAsync(notification);
            await unit.CommitAsync();
            if (!string.IsNullOrEmpty(patient.User.Email))
            {
                var filepath = $"{env.WebRootPath}/templates/ConfirmEmail.html";
                StreamReader reader = new StreamReader(filepath);
                var body = reader.ReadToEnd();
                reader.Close();
                body = body.Replace("[header]", message)
                    .Replace("[body]", "تم تأكيد موعدك بنجاح. نتمنى لك دوام الصحة.")
                    .Replace("[imageUrl]", "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763918337/icon-positive-vote-3_xfc5be.png\r\n");
                await emailSender.SendEmailAsync(patient.User.Email, "Sehaty", body);
                notificationDto.SentViaEmail = true;
            }
            await unit.CommitAsync();
            return true;
        }
    }
}

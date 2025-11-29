namespace Sehaty.Application.Services
{
    public class NotificationService(IMapper mapper, IUnitOfWork unit, ISmsSender smsSender, IWebHostEnvironment env, IEmailSender emailSender) : INotificationService
    {
        public async Task<bool> CreateNotificationForAppointmentCancellation(Appointment appointment)
        {
            string message =
                $"تم إلغاء موعدك مع الطبيب {appointment.Doctor.FirstName} {appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime:yyyy-MM-dd HH:mm}";

            return await CreateAppointmentNotificationAsync(
                appointment,
                "Appointment Canceled",
                message,
                "نود إبلاغكم بأنه تم إلغاء موعدكم بنجاح. إذا كنت بحاجة لحجز موعد بديل، يرجى التواصل معنا أو استخدام الموقع الإلكتروني.",
                "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763917652/icon-positive-vote-1_1_dpzjrw.png");
        }


        public async Task<bool> CreateNotificationForAppointmentConfirmation(Appointment appointment)
        {
            string message =
                $"تم تأكيد موعدك مع الطبيب {appointment.Doctor.FirstName} {appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime:yyyy-MM-dd HH:mm}";

            return await CreateAppointmentNotificationAsync(
                appointment,
                "Appointment Confirmed",
                message,
                "تم تأكيد موعدك بنجاح. نتمنى لك دوام الصحة.",
                "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763918337/icon-positive-vote-3_xfc5be.png");
        }


        private async Task CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = mapper.Map<Notification>(dto);
            await unit.Repository<Notification>().AddAsync(notification);
            await unit.CommitAsync();
        }

        private async Task SendEmailFromTemplateAsync(
            string email,
            string header,
            string bodyText,
            string imageUrl)
        {
            var filePath = Path.Combine(env.WebRootPath, "templates", "ConfirmEmail.html");

            var html = await File.ReadAllTextAsync(filePath);

            html = html
                .Replace("[header]", header)
                .Replace("[body]", bodyText)
                .Replace("[imageUrl]", imageUrl);

            await emailSender.SendEmailAsync(email, "Sehaty", html);
        }

        private async Task<bool> CreateAppointmentNotificationAsync(Appointment appointment,
            string title,
            string message,
            string emailBody,
            string imageUrl)
        {
            var patient = await unit.Repository<Patient>()
                                      .GetByIdAsync(appointment.PatientId);

            if (patient == null) return false;

            var dto = new CreateNotificationDto
            {
                UserId = appointment.PatientId,
                Title = title,
                Message = message,
                Priority = NotificationPriority.High,
                RelatedEntityType = "Appointment",
                RelatedEntityId = appointment.Id,
                NotificationType = NotificationType.Appointment,
                IsRead = false
            };

            await CreateNotificationAsync(dto);

            if (!string.IsNullOrEmpty(patient.User?.Email))
            {
                await SendEmailFromTemplateAsync(
                    patient.User.Email,
                    message,
                    emailBody,
                    imageUrl);

                dto.SentViaEmail = true;
                // إرسال SMS
                if (SendSmsAsync(patient.User?.PhoneNumber, message))
                {
                    dto.SentViaSMS = true;
                }
                await unit.CommitAsync();
            }

            return true;
        }

        private bool SendSmsAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            smsSender.SendSms(phoneNumber, message);
            return true;
        }

    }
}

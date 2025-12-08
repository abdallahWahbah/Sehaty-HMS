namespace Sehaty.Application.Services
{
    public class NotificationService(IMapper mapper, IUnitOfWork unit, ISmsSender smsSender, IWebHostEnvironment env, IEmailSender emailSender) : INotificationService
    {
        public async Task<bool> NotifyAppointmentCancellation(Appointment appointment)
        {
            var doctor = appointment.Doctor ?? await GetDoctorAsync(appointment.DoctorId);
            string message = $"تم إلغاء موعدك مع الطبيب {doctor.FirstName} {doctor.LastName} بتاريخ {appointment.AppointmentDateTime:yyyy-MM-dd HH:mm}";
            string emailBody = "نود إبلاغكم بأنه تم إلغاء موعدكم بنجاح. إذا كنت بحاجة لحجز موعد بديل، يرجى التواصل معنا أو استخدام الموقع الإلكتروني.";
            string imageUrl = "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763917652/icon-positive-vote-1_1_dpzjrw.png";

            return await CreateAppointmentNotificationAsync(appointment, "Appointment Canceled", message, emailBody, imageUrl);
        }

        public async Task<bool> NotifyAppointmentConfirmation(Appointment appointment)
        {
            var doctor = appointment.Doctor ?? await GetDoctorAsync(appointment.DoctorId);
            string message = $"تم تأكيد موعدك مع الطبيب {doctor.FirstName} {doctor.LastName} بتاريخ {appointment.AppointmentDateTime:yyyy-MM-dd HH:mm}";
            string emailBody = "تم تأكيد موعدك بنجاح. نتمنى لك دوام الصحة.";
            string imageUrl = "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763918378/icon-positive-vote-4_crzftt.png";

            return await CreateAppointmentNotificationAsync(appointment, "Appointment Confirmed", message, emailBody, imageUrl);
        }

        public async Task<bool> NotifyAppointmentUpdated(Appointment appointment)
        {
            var doctor = appointment.Doctor ?? await GetDoctorAsync(appointment.DoctorId);
            string message = $"تم تعديل موعدك مع الطبيب {doctor.FirstName} {doctor.LastName} إلى تاريخ {appointment.AppointmentDateTime:yyyy-MM-dd HH:mm}";

            string emailBody = "تم تعديل موعدك بنجاح، يرجى مراجعة التفاصيل الجديدة.";
            string imageUrl = "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763917652/icon-positive-vote-1_1_dpzjrw.png";

            return await CreateAppointmentNotificationAsync(
                appointment,
                "Appointment Updated",
                message,
                emailBody,
                imageUrl);
        }

        public async Task<bool> NotifyPrescriptionComplation(Prescription prescription)
        {

            var patient = prescription.Patient ?? await GetPatientAsync(prescription.PatientId);
            if (patient == null || patient.Id == 999999)
                return false;

            var doctor = await unit.Repository<Doctor>().GetByIdAsync(prescription.DoctorId);
            string message = $"تم تجهيز الروشته مع الطبيب {doctor.FirstName} {doctor.LastName} بتاريخ {prescription.DateIssued:yyyy-MM-dd}";

            string medicationsHtml = GenerateMedicationsHtml(prescription);
            //string emailBody = $"{prescription.SpecialInstructions}<br/>{medicationsHtml}";
            string imageUrl = "https://res.cloudinary.com/dl21kzp79/image/upload/f_png/v1763917652/icon-positive-vote-1_1_dpzjrw.png";

            var dto = new CreateNotificationDto
            {
                UserId = prescription.PatientId,
                Title = "Prescription Completed",
                Message = message,
                Priority = NotificationPriority.High,
                RelatedEntityType = "Prescription",
                RelatedEntityId = prescription.Id,
                NotificationType = NotificationType.Prescription,
                IsRead = false
            };

            await CreateNotificationAsync(dto);

            // Send Email
            if (!string.IsNullOrEmpty(patient.User?.Email))
            {
                await SendEmailFromTemplateAsync(templateName: "PrescriptionReady.html",
                    email: patient.User.Email,
                    header: message,
                    bodyText: prescription.SpecialInstructions,
                    imageUrl: imageUrl,
                    extraPlaceholders: new Dictionary<string, string>
                    {
                        ["[url]"] = $"https://localhost:7086/api/Prescriptions/prescriptions/{prescription.Id}/download",
                        ["[MedicationDeatails]"] = medicationsHtml,
                        ["[linkTitle]"] = "Download Prescription"
                    });
                dto.SentViaEmail = true;
            }

            //// Send SMS
            //if (SendSms(patient.User?.PhoneNumber, message))
            //{
            //    dto.SentViaSMS = true;
            //}

            await unit.CommitAsync();
            return true;
        }

        private async Task<Patient> GetPatientAsync(int patientId)
        {
            var spec = new PatientSpecifications(patientId);
            return (await unit.Repository<Patient>().GetByIdWithSpecAsync(spec));
        }

        private async Task<Doctor> GetDoctorAsync(int doctorId)
        {

            return (await unit.Repository<Doctor>().GetByIdAsync(doctorId));
        }

        private static string GenerateMedicationsHtml(Prescription prescription)
        {
            if (prescription.Medications == null || (prescription.Medications.Count == 0))
                return string.Empty;

            return string.Join("\n", prescription.Medications.Select(M =>
                $"<p><strong>{M.Medication.Name}</strong> — {M.Dosage}, {M.Frequency}, لمدة {M.Duration}</p>"));
        }

        private async Task CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = mapper.Map<Notification>(dto);
            await unit.Repository<Notification>().AddAsync(notification);
            await unit.CommitAsync();
        }

        private async Task SendEmailFromTemplateAsync(string templateName,
            string email,
            string header,
            string bodyText,
            string imageUrl,
            Dictionary<string, string> extraPlaceholders = null)
        {
            var filePath = Path.Combine(env.WebRootPath, "templates", templateName);

            var html = await File.ReadAllTextAsync(filePath);

            html = html
                .Replace("[header]", header)
                .Replace("[body]", bodyText)
                .Replace("[imageUrl]", imageUrl);

            if (extraPlaceholders != null)
            {
                foreach (var item in extraPlaceholders)
                    html = html.Replace(item.Key, item.Value);
            }

            await emailSender.SendEmailAsync(email, "Sehaty", html);
        }

        private async Task<bool> CreateAppointmentNotificationAsync(Appointment appointment,
            string title,
            string message,
            string emailBody,
            string imageUrl)
        {

            var patient = await GetPatientAsync(appointment.PatientId);

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
                await SendEmailFromTemplateAsync(templateName: "ConfirmEmail.html",
                    email: patient.User.Email,
                    header: message,
                    bodyText: emailBody,
                    imageUrl: imageUrl);

                dto.SentViaEmail = true;
            }
            //// إرسال SMS
            //if (SendSms(patient.User?.PhoneNumber, message))
            //{
            //    dto.SentViaSMS = true;
            //}
            await unit.CommitAsync();

            return true;
        }

        private bool SendSms(string phoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            smsSender.SendSms(phoneNumber, message);
            return true;
        }
    }
}

namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController(IUnitOfWork unit, IMapper map, IPrescriptionPdfService pdfService, IEmailSender emailSender, ISmsSender smsSender) : ApiBaseController
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = new PrescriptionSpecifications();
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            if (prescriptions != null)
                return Ok(map.Map<IEnumerable<GetPrescriptionsDto>>(prescriptions));
            return NotFound(new ApiResponse(404));
        }

        //get prescription by its id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = new PrescriptionSpecifications(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription != null)
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            return NotFound(new ApiResponse(404));
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("doctorprescriptions")]
        public async Task<IActionResult> GetByDoctorId()
        {
            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doctorId = unit.Repository<Doctor>().FindBy(D => D.UserId == doctorUserId).Select(D => D.Id).FirstOrDefault();
            var spec = new PrescriptionSpecifications(P => P.DoctorId == doctorId);

            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            var sortedprescriptions = prescriptions.OrderByDescending(p => p.DateIssued).ToList();
            if (sortedprescriptions.Count() > 0)
                return Ok(map.Map<IEnumerable<DoctorPrescriptionsDto>>(sortedprescriptions));
            return NotFound(new ApiResponse(404));
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("doctorprescriptions/{id}")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doctorId = unit.Repository<Doctor>().FindBy(D => D.UserId == doctorUserId).Select(D => D.Id).FirstOrDefault();
            var spec = new PrescriptionSpecifications(P => P.Id == id && P.DoctorId == doctorId);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription == null)
                return NotFound(new ApiResponse(404));
            return Ok(map.Map<GetPrescriptionsDto>(prescription));
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("patientprescriptions")]
        public async Task<IActionResult> GetByPatientId()
        {
            var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var spec = new PrescriptionSpecifications(P => P.PatientId == patientId);
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            var sortedprescriptions = prescriptions
                        .OrderByDescending(p => p.Status == PrescriptionStatus.Active)
                        .ThenByDescending(p => p.DateIssued)
                        .ToList();
            if (prescriptions != null)
                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(sortedprescriptions));
            return NotFound(new ApiResponse(404));
        }

        //[Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = map.Map<Prescription>(model);
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                prescription.DoctorId = int.Parse(doctorId);
                await unit.Repository<Prescription>().AddAsync(prescription);
                await unit.CommitAsync();

                if (prescription.PatientId.HasValue)
                {
                    var patient = await unit.Repository<Patient>().GetByIdAsync(prescription.PatientId.Value);
                    if (patient != null)
                    {
                        string message = $"تم تجهيز الروشته مع الطبيب {prescription.Doctor.FirstName} {prescription.Doctor.LastName} بتاريخ {prescription.DateIssued}";

                        var notificationDto = new CreateNotificationDto
                        {
                            UserId = prescription.PatientId,
                            Title = "Prescription Completed",
                            Message = message,
                            Priority = NotificationPriority.High,
                            RelatedEntityType = "Prescription",
                            RelatedEntityId = prescription.Id,
                            SentViaEmail = false,
                            SentViaSMS = false,
                            NotificationType = NotificationType.Prescription,
                            IsRead = false
                        };
                        var notification = map.Map<Notification>(notificationDto);
                        await unit.Repository<Notification>().AddAsync(notification);
                        await unit.CommitAsync();
                        if (!string.IsNullOrEmpty(patient.User.Email))
                        {
                            await emailSender.SendEmailAsync(patient.User.Email, "تم تجهيز الروشته", message);
                            notificationDto.SentViaEmail = true;
                        }
                        if (!string.IsNullOrEmpty(patient.User.PhoneNumber))
                        {
                            smsSender.SendSmsAsync(patient.User.PhoneNumber, message);
                            notificationDto.SentViaSMS = true;
                        }
                        await unit.CommitAsync();
                    }
                }


                return Ok(new { message = "Prescription created successfully", prescriptionId = prescription.Id });
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] CreatePrescriptionsDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = await unit.Repository<Prescription>().GetByIdAsync(id);
                if (prescription == null) return NotFound(new ApiResponse(404));
                map.Map(model, prescription);
                unit.Repository<Prescription>().Update(prescription);
                await unit.CommitAsync();
                return Ok();
            }
            return BadRequest(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await unit.Repository<Prescription>().GetByIdAsync(id);
            if (prescription == null) return NotFound(new ApiResponse(404));
            unit.Repository<Prescription>().Delete(prescription);
            await unit.CommitAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Patient,Doctor")]
        [HttpGet("prescriptions/{id}/download")]
        public async Task<IActionResult> DownloadPrescription(int id)
        {
            PrescriptionSpecifications spec = new PrescriptionSpecifications(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription is null)
                return NotFound(new ApiResponse(404));

            var pdfBytes = pdfService.GeneratePrescriptionPdf(prescription);
            return File(pdfBytes, "application/pdf", $"Prescription_{id}.pdf");
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("patient/{patientId}/history")]
        public async Task<IActionResult> GetPrescriptionHistoryForPatient(int patientId)
        {
            var spec = new PrescriptionSpecifications(P => P.PatientId == patientId);
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            if (prescriptions is not null && prescriptions.Any())
            {
                var sortedprescriptions = prescriptions
                       .OrderByDescending(p => p.DateIssued)
                       .ToList();
                return Ok(map.Map<IEnumerable<PrescriptionHistoryDto>>(sortedprescriptions));
            }
            return NotFound(new ApiResponse(404));
        }


    }

}

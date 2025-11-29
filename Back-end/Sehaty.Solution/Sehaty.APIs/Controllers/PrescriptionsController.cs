namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController(IUnitOfWork unit, IMapper map, IPrescriptionPdfService pdfService, INotificationService notificationService) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPrescriptionsDto>>> GetAll()
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
        [HttpGet("DoctorPrescriptions")]
        public async Task<IActionResult> GetByDoctor()
        {
            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doctorId = unit.Repository<Doctor>().FindBy(D => D.UserId == doctorUserId).Select(D => D.Id).FirstOrDefault();
            var spec = new PrescriptionSpecifications(P => P.DoctorId == doctorId);

            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            var sortedprescriptions = prescriptions.OrderByDescending(p => p.DateIssued).ToList();
            if (sortedprescriptions.Count > 0)
                return Ok(map.Map<IEnumerable<GetPrescriptionsDto>>(sortedprescriptions));
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
            var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var patientId = (await unit.Repository<Patient>().GetFirstOrDefaultAsync(P => P.UserId == patientUserId)).Id;

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

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(model.AppointmentId);

            if (appointment is null)
                return NotFound(new ApiResponse(404, "Appointment not found"));

            //if (appointment?.Status != AppointmentStatus.InProgress || appointment?.Status != AppointmentStatus.Completed)
            //    return BadRequest(new ApiResponse(400,
            //        "Oops! You can add a prescription only when the appointment is In Progress or Completed."));

            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var doctor = await unit.Repository<Doctor>().GetFirstOrDefaultAsync(D => D.UserId == doctorUserId);

            if (doctor is null)
                return NotFound(new ApiResponse(404, "Doctor not found"));

            var medicalRecord = await unit.Repository<MedicalRecord>()
                .GetFirstOrDefaultAsync(M => M.PatientId == model.PatientId);

            if (medicalRecord is null)
                return NotFound(new ApiResponse(404, "Medical record not found"));

            var prescription = map.Map<Prescription>(model);

            prescription.DoctorId = doctor.Id;
            prescription.MedicalRecordId = medicalRecord.Id;

            appointment.Status = AppointmentStatus.Completed;

            await unit.Repository<Prescription>().AddAsync(prescription);
            unit.Repository<Appointment>().Update(appointment);

            await unit.CommitAsync();


            await notificationService.NotifyPrescriptionComplation(prescription);

            return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, map.Map<GetPrescriptionsDto>(prescription));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto model)
        {
            if (ModelState.IsValid)
            {
                var spec = new PrescriptionSpecifications(id);
                var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
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

        //[Authorize(Roles = "Admin,Patient,Doctor")]
        [HttpGet("prescriptions/{id}/download")]
        public async Task<IActionResult> DownloadPrescription(int id)
        {
            PrescriptionSpecifications spec = new(id);
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

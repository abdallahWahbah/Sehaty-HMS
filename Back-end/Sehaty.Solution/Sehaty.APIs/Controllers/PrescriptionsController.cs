namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController(IUnitOfWork unit, IMapper map, IPrescriptionPdfService pdfService, INotificationService notificationService, IPrescriptionService prescriptionService) : ApiBaseController
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
            try
            {
                var prescription = await prescriptionService.GetPrescriptionDetailsAsync(id);
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("patientprescriptions")]
        public async Task<IActionResult> GetByPatientId()
        {
            try
            {
                var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var patientId = (await unit.Repository<Patient>().GetFirstOrDefaultAsync(P => P.UserId == patientUserId)).Id;

                var prescription = await prescriptionService.GetPatientPrescriptionsAsync(patientId);

                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(prescription));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            try
            {
                var prescription = await prescriptionService.CreatePrescriptionAsync(model);
                await notificationService.NotifyPrescriptionComplation(prescription);

                return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, map.Map<GetPrescriptionsDto>(prescription));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto model)
        {
            try
            {
                await prescriptionService.UpdatePrescriptionAsync(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {

            try
            {
                await prescriptionService.DeletePrescriptionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {

                return NotFound(new ApiResponse(404, ex.Message));
            }
        }

        //[Authorize(Roles = "Admin,Patient,Doctor")]
        [HttpGet("prescriptions/{id}/download")]
        public async Task<IActionResult> DownloadPrescription(int id)
        {
            PrescriptionSpecifications spec = new(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if (prescription is null)
                return NotFound(new ApiResponse(404));

            var pdfBytes = prescriptionService.GeneratePrescriptionPdf(prescription);
            return File(pdfBytes, "application/pdf", $"Prescription_{id}.pdf");
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("patient/{patientId}/history")]
        public async Task<IActionResult> GetPrescriptionHistoryForPatient(int patientId)
        {
            try
            {
                var prescription = await prescriptionService.GetPatientPrescriptionsAsync(patientId);

                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(prescription));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }


    }

}

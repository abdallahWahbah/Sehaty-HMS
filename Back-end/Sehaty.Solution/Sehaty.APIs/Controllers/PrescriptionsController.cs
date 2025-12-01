namespace Sehaty.APIs.Controllers
{

    public class PrescriptionsController(IUnitOfWork unit, IMapper map, INotificationService notificationService, IPrescriptionService prescriptionService) : ApiBaseController
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

            var result = await prescriptionService.GetPrescriptionDetailsAsync(id);
            if (result.IsSuccess)
            {
                var prescription = result.Data;
                return Ok(map.Map<GetPrescriptionsDto>(prescription));
            }
            return result.ToApiResponse();
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("patientprescriptions")]
        public async Task<IActionResult> GetRegisterdPatientPrescriptions()
        {

            var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var patientId = (await unit.Repository<Patient>().GetFirstOrDefaultAsync(P => P.UserId == patientUserId)).Id;
            var result = await prescriptionService.GetPatientPrescriptionsAsync(patientId);

            if (result.IsSuccess)
            {
                var prescription = result.Data;
                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(prescription));
            }
            return result.ToApiResponse();

        }

        //[Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionsDto model)
        {
            var result = await prescriptionService.CreatePrescriptionAsync(model);

            if (result.IsSuccess)
            {
                var prescription = result.Data;
                await notificationService.NotifyPrescriptionComplation(prescription);

                return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, map.Map<GetPrescriptionsDto>(prescription));
            }
            return result.ToApiResponse<Prescription>();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto model)
        {

            var result = await prescriptionService.UpdatePrescriptionAsync(id, model);

            return result.ToApiResponse();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {


            var result = await prescriptionService.DeletePrescriptionAsync(id);
            if (result.IsSuccess)
                return Ok();

            return result.ToApiResponse();

        }

        //[Authorize(Roles = "Admin,Patient,Doctor")]
        [HttpGet("prescriptions/{id}/download")]
        public async Task<IActionResult> DownloadPrescription(int id)
        {
            var result = await prescriptionService.GetPrescriptionPdfFile(id);
            if (result.IsSuccess)
            {
                var prescriptionPdfFile = result.Data;

                return File(prescriptionPdfFile, "application/pdf", $"Prescription_{id}.pdf");
            }
            return result.ToApiResponse();
        }

        //[Authorize(Roles = "Doctor,Admin")]
        [HttpGet("patient/{patientId}/history")]
        public async Task<IActionResult> GetPrescriptionHistoryForPatient(int patientId)
        {
            var result = await prescriptionService.GetPatientPrescriptionsAsync(patientId);
            if (result.IsSuccess)
            {
                var prescription = result.Data;

                return Ok(map.Map<IEnumerable<PatientPrescriptionsDto>>(prescription));
            }
            return result.ToApiResponse();
        }

    }

}

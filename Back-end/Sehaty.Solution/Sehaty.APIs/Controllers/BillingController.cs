namespace Sehaty.APIs.Controllers
{
    public class BillingController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {
        [HttpGet("GetAllForPatient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAllBillingsForPatient([FromQuery] int id)
        {
            try
            {
                var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var patient = await unit.Repository<Patient>()
                    .GetFirstOrDefaultAsync(P => P.UserId == patientUserId);

                if (patient == null)
                    return NotFound(new ApiResponse(404, "Patient not found"));

                if (patient.Id != id)
                    return BadRequest(new ApiResponse(400, "you don't have access to This Billing"));

                var spec = new BillingSpec(b => b.PatientId == patient.Id && b.Status != BillingStatus.Pending);
                var allBillings = await unit.Repository<Billing>()
                    .GetAllWithSpecAsync(spec);

                if (!allBillings.Any())
                    return Ok(new ApiResponse(200, "No billing history found for patient"));

                var data = mapper.Map<List<BillingReadDto>>(allBillings);
                return Ok(data);
            }
            catch (FormatException)
            {
                return BadRequest(new ApiResponse(400, "Invalid user Authentication "));
            }
        }

    }
}

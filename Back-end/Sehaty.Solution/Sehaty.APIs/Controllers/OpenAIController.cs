using Sehaty.Application.Dtos.AiDto;

namespace Sehaty.APIs.Controllers
{
    [Authorize(Roles = "Patient")]
    public class OpenAIController(IOpenAIService aiService, IUnitOfWork unit) : ApiBaseController
    {
        [HttpGet("analyze/{prescriptionId}")]
        public async Task<ActionResult<PrescriptionAnalysisResponseDto>> AnalyzePrescription(int prescriptionId)
        {
            try
            {
                var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var patient = await unit.Repository<Patient>()
                    .GetFirstOrDefaultAsync(P => P.UserId == patientUserId);

                if (patient == null)
                    return NotFound(new ApiResponse(404, "Patient not found"));

                var prescription = await unit.Repository<Prescription>()
                    .GetFirstOrDefaultAsync(P => P.Id == prescriptionId && P.PatientId == patient.Id);

                if (prescription == null)
                    return NotFound(new ApiResponse(404, "Prescription not found or you don't have access to it"));

                var analysis = await aiService.AnalyzePrescriptionAsync(prescriptionId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }
    }
}

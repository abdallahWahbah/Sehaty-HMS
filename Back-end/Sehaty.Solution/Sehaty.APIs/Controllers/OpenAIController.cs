using Sehaty.Application.Dtos.AiDto;
using Sehaty.Application.Dtos.OpenAIDto;

namespace Sehaty.APIs.Controllers
{

    public class OpenAIController(IOpenAIService aiService,IUnitOfWork unit) : ApiBaseController
    {
        [HttpGet("analyze/{prescriptionId}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PrescriptionAnalysisResponseDto>> AnalyzePrescription(int prescriptionId)
        {
            try
            {
                var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var patient = await unit.Repository<Patient>()
                    .GetFirstOrDefaultAsync(P => P.UserId == patientUserId);

                if(patient == null)
                    return NotFound(new ApiResponse(404,"Patient not found"));

                var prescription = await unit.Repository<Prescription>()
                    .GetFirstOrDefaultAsync(P => P.Id == prescriptionId && P.PatientId == patient.Id);

                if(prescription == null)
                    return NotFound(new ApiResponse(404,"Prescription not found or you don't have access to it"));

                var analysis = await aiService.AnalyzePrescriptionAsync(prescriptionId);
                return Ok(analysis);
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse(400,ex.Message));
            }
        }

        [HttpGet("analyze-patient-history/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PatientHistoryAnalysisResponseDto>> AnalyzePatientHistory(int patientId)
        {
            try
            {
                var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var doctor = await unit.Repository<Doctor>()
                    .GetFirstOrDefaultAsync(d => d.UserId == doctorUserId);

                if(doctor == null)
                    return NotFound(new ApiResponse(404,"Doctor not found"));

                var analysis = await aiService.AnalyzePatientHistoryAsync(patientId);

                if(!analysis.IsSuccess)
                    return StatusCode((int) analysis.ErrorType,new ApiResponse((int) analysis.ErrorType,analysis.Error));

                return Ok(analysis);
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse(400,ex.Message));
            }
        }
    }
}

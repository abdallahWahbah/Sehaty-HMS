using Sehaty.Application.Dtos.AiDto;
using Sehaty.Application.Dtos.OpenAIDto;
using Sehaty.Application.Dtos.OpenAIDto.SuggestAppointmentBySymptomsDto;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IOpenAIService
    {
        Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId);
        Task<Result<PatientHistoryAnalysisResponseDto>> AnalyzePatientHistoryAsync(int patientId, int doctorId);
        Task<Result<SymptomsAnalysisResponseDto>> AnalyzeSymptomsAndSuggestAppointmentAsync(SymptomsAnalysisRequestDto request);
    }
}

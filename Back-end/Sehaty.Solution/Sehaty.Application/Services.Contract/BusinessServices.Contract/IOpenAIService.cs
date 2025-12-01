using Sehaty.Application.Dtos.AiDto;
using Sehaty.Application.Dtos.OpenAIDto;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IOpenAIService
    {
        Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId);
        public Task<Result<PatientHistoryAnalysisResponseDto>> AnalyzePatientHistoryAsync(int patientId, int doctorId);
    }
}

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IOpenAIService
    {
        Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId);
        Task<Result<PatientHistoryAnalysisResponseDto>> AnalyzePatientHistoryAsync(int patientId, int doctorId);
        Task<Result<SymptomsAnalysisResponseDto>> AnalyzeSymptomsAndSuggestAppointmentAsync(SymptomsAnalysisRequestDto request);
    }
}

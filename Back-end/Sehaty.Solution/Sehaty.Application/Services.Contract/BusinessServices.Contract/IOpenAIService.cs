using Sehaty.Application.Dtos.AiDto;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IOpenAIService
    {
        Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId);
    }
}

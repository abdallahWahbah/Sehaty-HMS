namespace Sehaty.Application.Dtos.OpenAIDto.SuggestAppointmentBySymptomsDto
{
    public class SymptomsAnalysisResponseDto
    {
        public string AnalyzedSymptoms { get; set; }
        public string SuggestedSpecialization { get; set; }
        public List<SuggestedSlotDto> AvailableSlots { get; set; } = new();
    }
}

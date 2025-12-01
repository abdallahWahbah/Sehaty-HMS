namespace Sehaty.Application.Dtos.OpenAIDto
{
    public class PatientHistoryAnalysisResponseDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int TotalPrescriptions { get; set; }
        public string AISummary { get; set; }
        public List<RecordSummaryDto> Records { get; set; } = new();
    }
}

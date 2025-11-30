namespace Sehaty.Application.Dtos.AiDto
{
    public class PrescriptionAnalysisResponseDto
    {
        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime DateIssued { get; set; }
        public string AnalysisResult { get; set; }
        public List<MedicationAnalysisDto> MedicationsAnalysis { get; set; }
        public string GeneralInstructions { get; set; }
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    }
}

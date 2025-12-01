namespace Sehaty.Application.Dtos.OpenAIDto
{
    public class RecordSummaryDto
    {
        public int RecordId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; }
        public string Diagnosis { get; set; }
        public string Symptoms { get; set; }
        public string TreatmentPlan { get; set; }
        public List<string> Medications { get; set; } = new();
    }
}

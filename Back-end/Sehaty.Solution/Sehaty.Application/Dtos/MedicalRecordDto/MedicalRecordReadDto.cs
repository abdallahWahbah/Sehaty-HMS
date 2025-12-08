namespace Sehaty.Application.Dtos.MedicalRecordDto
{
    public class MedicalRecordReadDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public int? BpSystolic { get; set; }
        public int? BpDiastolic { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        public string VitalBp { get; set; }
        public string Notes { get; set; }
        public RecordType RecordType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsFinialize { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
    }
}

using Sehaty.Core.Entites;

namespace Sehaty.Application.Dtos
{
    public class MedicalRecordDto
    {
        public int AppointmentId { get; set; }
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
        public RecordType RecordType { get; set; } = RecordType.Diagnosis;
        public DateTime? CreatedAt { get; set; }
    }
}

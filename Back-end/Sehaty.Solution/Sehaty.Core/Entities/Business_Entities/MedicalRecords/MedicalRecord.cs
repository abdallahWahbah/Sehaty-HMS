namespace Sehaty.Core.Entities.Business_Entities.MedicalRecords
{
    public enum RecordType
    {
        Diagnosis,
        LabResult,
        Imaging,
        FollowUp,
        Procedure
    }
    public class MedicalRecord : BaseEntity
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
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

        // Navigation Property to Prescriptions
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    }
}
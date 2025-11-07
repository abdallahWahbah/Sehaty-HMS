using Sehaty.Core.Entites;

namespace Sehaty.Core.Entities.Business_Entities
{
    public enum PrescriptionStatus
    {
        Active,
        Completed
    }

    public class Prescription : BaseEntity
    {
        public PrescriptionStatus Status { get; set; }
        public string DigitalSignature { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime DateIssued { get; set; }
        public int AppointmentId { get; set; } // Foreign key to Appointment
        public Appointment Appointment { get; set; }
        public int? MedicalRecordId { get; set; } // Foreign key to MedicalRecord
        public MedicalRecord MedicalRecord { get; set; }
        public int? PatientId { get; set; } // Foreign key to patient
        public Patient Patient { get; set; }
        public int? DoctorId { get; set; } // Foreign key to doctor
        public Doctor Doctor { get; set; }
        public List<PrescriptionMedications> Medications { get; set; }
    }
}

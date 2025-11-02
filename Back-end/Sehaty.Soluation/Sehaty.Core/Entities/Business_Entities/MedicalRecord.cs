using Sehaty.Core.Entities.Business_Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sehaty.Core.Entites
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
        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "nvarchar(max)")]
        public string Symptoms { get; set; }
        [Column(TypeName = "nvarchar(max)")]

        public string Diagnosis { get; set; }
        public int TreatmentPlan { get; set; }
        public int? BpSystolic { get; set; }
        public int? BpDiastolic { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        [Column(TypeName = "nvarchar(20)")]

        [MaxLength(20)]
        public string VitalBp { get; set; }
        [Column(TypeName = "nvarchar(max)")]

        public string Notes { get; set; }
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        [EnumDataType(typeof(RecordType))]
        public RecordType RecordType { get; set; } = RecordType.Diagnosis;
        public DateTime? CreatedAt { get; set; }

        // Navigation Property to Prescriptions
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    }
}

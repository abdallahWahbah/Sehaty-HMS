using Sehaty.Core.Entities.Business_Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ForeignKey(nameof(Appointments))]
        public int Appointment_ID { get; set; }
        public Appointment Appointments { get; set; }
        public DateTime Record_Date { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "nvarchar(max)")]
        public string? Symptoms { get; set; }
        [Column(TypeName = "nvarchar(max)")]

        public string Diagnosis { get; set; }
        public int Treatment_Plan { get; set; }
        public int? BP_Systolic { get; set; }
        public int? BP_Diastolic { get; set; }
        public decimal? Temperature { get; set; }
        public int? Heart_Rate { get; set; }
        public decimal? Weight { get; set; }
        [Column(TypeName = "nvarchar(20)")]

        [MaxLength(20)]
        public string? Vital_BP { get; set; }
        [Column(TypeName = "nvarchar(max)")]

        public string? Notes { get; set; }
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        [EnumDataType(typeof(RecordType))]
        public RecordType Record_Type { get; set; } = RecordType.Diagnosis;
        public DateTime? Created_At { get; set; }

        // Navigation Property to Prescriptions
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    }
}

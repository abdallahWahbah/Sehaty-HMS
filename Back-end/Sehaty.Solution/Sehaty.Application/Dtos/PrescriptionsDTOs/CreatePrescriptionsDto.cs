using Sehaty.Core.Entities.Business_Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class CreatePrescriptionsDto
    {
        
        [Required]
        [MaxLength(200)]
        public string MedicationName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; }
        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; }
        [MaxLength(100)]
        public string Duration { get; set; }
        public PrescriptionStatus Status { get; set; }
        [MaxLength(255)]
        public string DigitalSignature { get; set; }
        [MaxLength(200)]
        public string SpecialInstructions { get; set; }
        public DateTime DateIssued { get; set; }
        public int AppointmentId { get; set; }
        public int? MedicalRecordId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
    }
}

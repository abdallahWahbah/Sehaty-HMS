using Sehaty.Application.Dtos.MedicationDTOs;
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
        public PrescriptionStatus Status { get; set; }
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public int? MedicalRecordId { get; set; }

        [MaxLength(200)]
        public string SpecialInstructions { get; set; }
        public string DigitalSignature { get; set; }


        [Required]
        public List<MedicationDto> Medications { get; set; }
    }
}

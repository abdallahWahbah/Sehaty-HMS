using Sehaty.Application.Dtos.MedicationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class PatientPrescriptionsDto
    {
        public int PrescriptionId { get; set; }

        public string DoctorName { get; set; }

        public DateTime DateIssued { get; set; }

        public string Status { get; set; }

        public string DoctorNotes { get; set; }

        public List<MedicationDto> Medications { get; set; }
    }
}

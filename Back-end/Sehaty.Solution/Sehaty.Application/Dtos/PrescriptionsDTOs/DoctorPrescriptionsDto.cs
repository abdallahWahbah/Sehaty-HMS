using Sehaty.Core.Entities.Business_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class DoctorPrescriptionsDto
    {
        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateIssued { get; set; }
        public PrescriptionStatus Status { get; set; }
    }
}

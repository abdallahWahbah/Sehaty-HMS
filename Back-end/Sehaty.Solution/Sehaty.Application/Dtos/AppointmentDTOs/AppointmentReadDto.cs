using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }

        // Nested related info
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string Notes { get; set; }

    }
}

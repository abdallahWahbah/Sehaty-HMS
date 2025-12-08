using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.OpenAIDto
{
    public class PrescriptionAlternativesResponseDto
    {
        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string AlternativesAndSideEffects { get; set; } // AI Response
    }

}

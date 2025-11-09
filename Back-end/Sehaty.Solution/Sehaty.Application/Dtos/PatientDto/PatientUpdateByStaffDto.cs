using Sehaty.Core.Entites;
using System.ComponentModel.DataAnnotations;

namespace Sehaty.Application.Dtos.PatientDto
{
    public class PatientUpdateByStaffDto
    {
        public string MRN { get; set; }
        [Required]
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        [EnumDataType(typeof(PatientStatus))]
        public PatientStatus Status { get; set; }

    }
}

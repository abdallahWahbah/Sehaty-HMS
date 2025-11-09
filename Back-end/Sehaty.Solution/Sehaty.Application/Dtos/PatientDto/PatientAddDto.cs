using Sehaty.Core.Entites;
using System.ComponentModel.DataAnnotations;

namespace Sehaty.Application.Dtos.PatientDto
{
    public class PatientAddDto
    {
        public string MRN { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }

        [EnumDataType(typeof(PatientStatus))]
        public PatientStatus Status { get; set; } = PatientStatus.Active;
        public int UserId { get; set; }
    }
}

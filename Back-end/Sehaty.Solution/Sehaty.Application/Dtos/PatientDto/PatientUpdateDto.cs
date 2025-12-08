namespace Sehaty.Application.Dtos.PatientDto
{
    public class PatientUpdateDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName { get; set; }
        [Phone]
        [RegularExpression(@"^\+20(10|11|12|15)\d{8}$")]
        public string EmergencyContactPhone { get; set; }

    }
}

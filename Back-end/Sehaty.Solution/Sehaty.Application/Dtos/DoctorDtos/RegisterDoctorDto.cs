namespace Sehaty.Application.Dtos.DoctorDtos
{
    public class RegisterDoctorDto
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\+20(10|11|12|15)\d{8}$")]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [EnumDataType(typeof(LanguagePreferenceEnum))]
        public LanguagePreferenceEnum LanguagePreference { get; set; } = LanguagePreferenceEnum.Arabic;

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; }

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; }
        [Required]
        public int DetectionPrice { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        //public IFormFile ProfilePhoto { get; set; }
        public string AvailabilityNotes { get; set; }
        public int DepartmentId { get; set; }
    }
}

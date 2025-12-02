namespace Sehaty.Application.Dtos.PatientDto
{
    public class RegisterPatientDto
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
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }
        [RegularExpression(@"^(2|3)\d{13}$",
            ErrorMessage = "National ID must be 14 digits and start with 2 or 3")]
        public string NationalId { get; set; }
        [MaxLength(3)]
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName { get; set; }
        [Phone]
        [RegularExpression(@"^\+20(10|11|12|15)\d{8}$")]
        public string EmergencyContactPhone { get; set; }
    }

}

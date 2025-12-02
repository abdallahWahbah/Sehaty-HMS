namespace Sehaty.Application.Dtos.PatientDto
{
    public class GetRegisterPatientDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LanguagePreferenceEnum LanguagePreference { get; set; } = LanguagePreferenceEnum.Arabic;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}

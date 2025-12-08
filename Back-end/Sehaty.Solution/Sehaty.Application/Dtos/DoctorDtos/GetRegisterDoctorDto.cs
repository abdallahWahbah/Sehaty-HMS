namespace Sehaty.Application.Dtos.DoctorDtos
{
    public class GetRegisterDoctorDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LanguagePreferenceEnum LanguagePreference { get; set; } = LanguagePreferenceEnum.Arabic;
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        //public string ProfilePhotoUrl { get; set; }
        public string AvailabilityNotes { get; set; }
        public int CancelledAppointmentsCount { get; set; } = 0;
        public int DetectionPrice { get; set; }
        public int UserId { get; set; }
        public int? DepartmentId { get; set; }
    }
}

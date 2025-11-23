namespace Sehaty.Application.Dtos.PatientDto
{
    public class GetPatientDto
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        public string Address { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public PatientStatus Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
    }
}

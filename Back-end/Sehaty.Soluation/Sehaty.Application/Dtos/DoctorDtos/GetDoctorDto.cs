namespace Sehaty.Application.Dtos.DoctorDtos
{
    public class GetDoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string AvailabilityNotes { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
    }
}

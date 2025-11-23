namespace Sehaty.Application.Dtos.DoctorDtos
{
    public class DoctorAddUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; }

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        //public IFormFile ProfilePhoto { get; set; }
        public string AvailabilityNotes { get; set; }
        public int UserId { get; set; }
        public int? DepartmentId { get; set; }
    }
}

namespace Sehaty.Application.Dtos.DepartmentDtos
{
    public class GetDepartmentDto
    {
        public int Id { get; set; }
        public string En_Name { get; set; }
        public string Ar_Name { get; set; }
        public string Description { get; set; }
        public ICollection<DoctorInDepartmentDto> Doctors { get; set; }
    }
    public class DoctorInDepartmentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        //public string ProfilePhoto { get; set; }
    }
}

using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Core.Entites
{
    public class Doctor : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public string Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        public string ProfilePhoto { get; set; }
        public string AvailabilityNotes { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<DoctorAvailabilitySlot> DoctorAvailabilitySlots { get; set; }
        public List<Prescription> Prescriptions { get; set; }
    }
}

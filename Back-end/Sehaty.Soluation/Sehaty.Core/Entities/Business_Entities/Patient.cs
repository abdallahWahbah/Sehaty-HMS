using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Entities.User_Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sehaty.Core.Entites
{
    public enum PatientStatus
    {
        Active,
        Discharged,
        Readmitted
    }
    public class Patient : BaseEntity
    {
        public string MRN { get; set; }
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

        // NAVIGATION PROPERTIES

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<Billing> Billings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sehaty.Core.Entities.Business_Entities;

namespace Sehaty.Core.Entites
{
    public class Appointment : BaseEntity
    {
        public int Id { get; set; } 
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }

        public DateTime? BookingDateTime { get; set; }
        public DateTime? ConfirmationDateTime { get; set; }
        public string CancellationReason { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<Billing> Billings { get; set; }
        public List<Feedback> Feedbacks { get; set; }
    }
}

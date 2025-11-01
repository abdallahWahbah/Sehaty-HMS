using Sehaty.Core.Entites;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.Business_Entities
{
	public enum PrescriptionStatus
	{
		Active,
		Completed
	}

	public class Prescription : BaseEntity
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
		public string Frequency { get; set; }
		public string Duration { get; set; }
		public PrescriptionStatus Status { get; set; }
		public string DigitalSignature { get; set; }
		public string SpecialInstructions { get; set; }
		public DateTime DateIssued { get; set; }
		public int AppointmentId { get; set; } // Foreign key to Appointment
		public Appointment Appointment { get; set; }
		public int? RecordId { get; set; } // Foreign key to MedicalRecord
		public MedicalRecord MedicalRecord { get; set; }
		public int? PatientId { get; set; } // Foreign key to patient
		public Patient Patient { get; set; }
		public int? DoctorId { get; set; } // Foreign key to doctor
		public Doctor Doctor { get; set; }
	}
}

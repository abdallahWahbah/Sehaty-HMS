namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class GetPrescriptionsDto
    {

        public string Id { get; set; }

        public PrescriptionStatus Status { get; set; }
        public string DigitalSignature { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime DateIssued { get; set; }
        public int AppointmentId { get; set; }
        public int? MedicalRecordId { get; set; }
        public string MedicalRecordNotes { get; set; }
        public int? PatientId { get; set; }
        public string PatiantName { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string LicenseNumber { get; set; }
        public List<MedicationDto> Medications { get; set; }
    }
}

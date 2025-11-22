namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class CreatePrescriptionsDto
    {
        public PrescriptionStatus Status { get; set; }
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public int? MedicalRecordId { get; set; }

        [MaxLength(200)]
        public string SpecialInstructions { get; set; }
        public string DigitalSignature { get; set; }


        [Required]
        public List<MedicationDto> Medications { get; set; }
    }
}

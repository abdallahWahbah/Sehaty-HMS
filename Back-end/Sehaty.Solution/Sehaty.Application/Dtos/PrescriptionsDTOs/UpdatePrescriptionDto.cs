namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class UpdatePrescriptionDto
    {
        public PrescriptionStatus Status { get; set; }
        [MaxLength(200)]
        public string SpecialInstructions { get; set; }
        public string DigitalSignature { get; set; }


        [Required]
        public List<MedicationDto> Medications { get; set; }
    }
}

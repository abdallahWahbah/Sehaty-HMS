namespace Sehaty.Application.Dtos.MedicationDTOs
{
    public class MedicationDto
    {
        [Required]
        [MaxLength(100)]
        public string MedicationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; }

        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; }

        [Required]
        [MaxLength(100)]
        public string Duration { get; set; }
    }
}

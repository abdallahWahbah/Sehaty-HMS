namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class PrescriptionHistoryDto
    {
        public int PrescriptionId { get; set; }
        public DateTime DateIssued { get; set; }
        public string DoctorName { get; set; }
        public List<string> MedicationNames { get; set; } = new List<string>();
        public string Status { get; set; }
    }
}

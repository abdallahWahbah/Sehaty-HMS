namespace Sehaty.Application.Dtos.PrescriptionsDTOs
{
    public class DoctorPrescriptionsDto
    {
        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateIssued { get; set; }
        public PrescriptionStatus Status { get; set; }
    }
}

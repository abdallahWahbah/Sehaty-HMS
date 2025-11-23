namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
    }
}

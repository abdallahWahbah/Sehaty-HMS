namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonForVisit { get; set; }
        public int DurationMinutes { get; set; }
        public AppointmentStatus Status { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
    }
}

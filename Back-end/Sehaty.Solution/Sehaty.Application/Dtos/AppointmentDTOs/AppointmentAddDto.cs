namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentAddDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public int? DurationMinutes { get; set; }
        public string ReasonForVisit { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }


    }
    public class RescheduleAppointmentDto
    {
        public DateTime NewAppointmentDateTime { get; set; }
    }
}

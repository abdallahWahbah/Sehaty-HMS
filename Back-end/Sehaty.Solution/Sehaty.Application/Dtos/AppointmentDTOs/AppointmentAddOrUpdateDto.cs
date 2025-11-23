namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentAddOrUpdateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public DateTime? ConfirmationDateTime { get; set; }
        public string CancellationReason { get; set; }


    }
}

namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentAddForAnonymousDto
    {
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonForVisit { get; set; }
    }
}

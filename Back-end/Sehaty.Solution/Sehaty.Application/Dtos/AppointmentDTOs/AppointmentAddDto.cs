namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentAddDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonForVisit { get; set; }

    }

}

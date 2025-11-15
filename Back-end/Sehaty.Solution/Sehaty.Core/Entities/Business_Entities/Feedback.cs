using Sehaty.Core.Entities.Business_Entities.Appointments;

namespace Sehaty.Core.Entites
{
    public class Feedback : BaseEntity
    {
        // Range (1,5)
        public int Rating { get; set; }
        public string Comments { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}

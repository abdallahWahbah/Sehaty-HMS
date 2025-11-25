
namespace Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots
{
    public class DoctorAppointmentSlot : BaseEntity
    {
        public int DoctorId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public int? AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }

}

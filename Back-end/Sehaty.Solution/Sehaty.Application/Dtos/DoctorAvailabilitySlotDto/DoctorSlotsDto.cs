namespace Sehaty.Application.Dtos.DoctorAvailabilitySlotDto
{

    public class DoctorSlotsDto
    {
        public int DoctorId { get; set; }
        public WeekDays DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDuration { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly Date { get; set; }
    }
}

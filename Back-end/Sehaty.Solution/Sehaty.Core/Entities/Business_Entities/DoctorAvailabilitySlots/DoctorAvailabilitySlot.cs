namespace Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots
{
    [Flags]
    public enum WeekDays
    {
        None = 0,
        Saturday = 1 << 0, // 1
        Sunday = 1 << 1, // 2
        Monday = 1 << 2, // 4
        Tuesday = 1 << 3, // 8
        Wednesday = 1 << 4, // 16
        Thursday = 1 << 5, // 32
        Friday = 1 << 6  // 64
    }

    public class DoctorAvailabilitySlot : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public WeekDays DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly? Date { get; set; }
        public bool AvailableFlag { get; set; }
    }
}

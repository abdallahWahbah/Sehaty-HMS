using System.ComponentModel.DataAnnotations.Schema;

namespace Sehaty.Core.Entites
{
    [Flags]
    public enum WeekDays
    {
        None = 0,
        Sunday = 1 << 0, 
        Monday = 1 << 1, 
        Tuesday = 1 << 2, 
        Wednesday = 1 << 3, 
        Thursday = 1 << 4, 
        Friday = 1 << 5, 
        Saturday = 1 << 6 
    }

    public class DoctorAvailabilitySlot : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public WeekDays DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly Date { get; set; }
        public bool AvailableFlag { get; set; }
    }
}

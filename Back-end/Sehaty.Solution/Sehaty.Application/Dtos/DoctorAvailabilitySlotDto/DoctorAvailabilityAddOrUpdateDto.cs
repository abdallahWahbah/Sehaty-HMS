namespace Sehaty.Application.Dtos.DoctorAvailabilitySlotDto
{
    public class DoctorAvailabilityAddOrUpdateDto
    {
        [Required]
        public int DoctorId { get; set; }
        [Required]
        [EnumDataType(typeof(DayOfWeek))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly Date { get; set; }
        public bool AvailableFlag { get; set; }
    }
}

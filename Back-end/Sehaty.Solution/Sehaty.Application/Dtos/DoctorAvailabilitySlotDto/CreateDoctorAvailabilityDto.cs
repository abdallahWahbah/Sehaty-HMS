using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Application.Dtos.DoctorAvailabilitySlotDto
{
    public class CreateDoctorAvailabilityDto
    {
        public int DoctorId { get; set; }
        public WeekDays Days { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly? Date { get; set; }
    }

}

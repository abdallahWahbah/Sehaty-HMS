using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

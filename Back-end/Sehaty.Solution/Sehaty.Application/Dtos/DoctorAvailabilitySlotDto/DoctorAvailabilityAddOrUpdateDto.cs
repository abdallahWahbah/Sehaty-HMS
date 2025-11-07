using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

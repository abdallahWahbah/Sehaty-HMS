using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class AppointmentUpdateDto
    {
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public DateTime? ConfirmationDateTime { get; set; }
        public string CancellationReason { get; set; }
    }
}

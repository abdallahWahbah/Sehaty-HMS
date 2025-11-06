using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Dtos.AppointmentDtos
{
    public class AppointmentUpdateDto
    {
        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Range(15, 240)]
        public int DurationMinutes { get; set; }

        [StringLength(200)]
        public string ReasonForVisit { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public TimeSpan ScheduledTime { get; set; }

        public DateTime? ConfirmationDateTime { get; set; }

        [StringLength(200)]
        public string CancellationReason { get; set; }
    }
}

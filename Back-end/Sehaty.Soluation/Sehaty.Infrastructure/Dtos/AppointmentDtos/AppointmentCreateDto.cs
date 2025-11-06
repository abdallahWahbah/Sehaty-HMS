using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Infrastructure.Dtos.AppointmentDtos
{
    public class AppointmentCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Range(15, 240, ErrorMessage = "Duration must be between 15 and 240 minutes.")]
        public int DurationMinutes { get; set; }

        [Required, StringLength(200)]
        public string ReasonForVisit { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public TimeSpan ScheduledTime { get; set; }
    }
}

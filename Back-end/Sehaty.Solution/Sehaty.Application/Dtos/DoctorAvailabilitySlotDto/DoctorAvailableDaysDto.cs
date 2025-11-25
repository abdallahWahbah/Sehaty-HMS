using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.DoctorAvailabilitySlotDto
{
    public class DoctorAvailableDaysDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public List<AvailabilityScheduleDto> RecurringSchedule { get; set; }
        public List<SpecificDateScheduleDto> SpecificDates { get; set; }
        public string AvailableDaysString { get; set; }
    }

    public class AvailabilityScheduleDto
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Available { get; set; }
    }

    public class SpecificDateScheduleDto
    {
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class BookSlotResponseDto
    {
        public int AppointmentId { get; set; }
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; }
    }


    public class SlotDetailsDto
    {
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateOnly Date { get; set; }
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string TimeRange { get; set; }
        public bool IsBooked { get; set; }
        public AppointmentDetailsDto Appointment { get; set; }
    }
    public class AppointmentDetailsDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string ReasonForVisit { get; set; }
        public string Status { get; set; }
        public DateTime? BookingDateTime { get; set; }
    }

    public class GenerateSlotsDto
    {
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public DateOnly Date { get; set; }

    }
}

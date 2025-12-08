namespace Sehaty.Application.Dtos.DoctorAvailabilitySlotDto
{

    public class GetAvailableSlotsRequestDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateOnly Date { get; set; }
    }

    public class AvailableSlotDto
    {
        public int SlotId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string TimeRange { get; set; }
        public bool IsBooked { get; set; }
    }

    public class GetAvailableSlotsResponseDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateOnly Date { get; set; }
        public string DayOfWeek { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int BookedSlots { get; set; }
        public List<AvailableSlotDto> Slots { get; set; }
    }
    public class BookSlotDto
    {
        [Required]
        public int SlotId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public string ReasonForVisit { get; set; }
    }





}


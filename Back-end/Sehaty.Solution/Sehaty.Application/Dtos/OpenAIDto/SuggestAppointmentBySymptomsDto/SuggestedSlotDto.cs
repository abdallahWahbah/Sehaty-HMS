namespace Sehaty.Application.Dtos.OpenAIDto.SuggestAppointmentBySymptomsDto
{
    public class SuggestedSlotDto
    {
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal ConsultationFee { get; set; }
    }
}

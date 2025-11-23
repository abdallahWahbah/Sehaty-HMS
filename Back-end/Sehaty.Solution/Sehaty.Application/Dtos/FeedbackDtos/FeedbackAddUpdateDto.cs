namespace Sehaty.Application.Dtos.FeedbackDtos
{
    public class FeedbackAddUpdateDto
    {
        [Required]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string Comments { get; set; }
        public bool IsAnonymous { get; set; }
        public int AppointmentId { get; set; }
    }
}

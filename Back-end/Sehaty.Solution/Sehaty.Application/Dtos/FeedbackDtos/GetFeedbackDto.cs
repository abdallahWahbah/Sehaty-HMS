namespace Sehaty.Application.Dtos.FeedbackDtos
{
    public class GetFeedbackDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int AppointmentId { get; set; }
        public DateTime AppointmentDateTime { get; set; }

    }
}

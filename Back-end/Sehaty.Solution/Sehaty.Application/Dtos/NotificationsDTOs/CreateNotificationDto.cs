namespace Sehaty.Application.Dtos.NotificationsDTOs
{
    public class CreateNotificationDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        [Required]
        [EnumDataType(typeof(NotificationPriority))]
        public NotificationPriority Priority { get; set; }
        [EnumDataType(typeof(NotificationType))]
        public NotificationType? NotificationType { get; set; }
        [Required]

        public string RelatedEntityType { get; set; }
        [Required]
        public int? RelatedEntityId { get; set; }
        [Required]
        public bool SentViaEmail { get; set; }
        [Required]
        public bool SentViaSMS { get; set; }
        public int? UserId { get; set; }

    }
}

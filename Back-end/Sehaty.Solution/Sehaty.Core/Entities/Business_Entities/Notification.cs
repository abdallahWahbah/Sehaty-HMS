using Sehaty.Core.Entities.User_Entities;

namespace Sehaty.Core.Entites
{
    public enum NotificationType
    {
        Appointment,
        Prescription,
        Payment,
        Result,
        General
    }
    public enum NotificationPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }

    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }//how we would track if notification is read or not????
        public bool IsRead { get; set; } //how we would track if notification is read or not????
        public NotificationPriority? Priority { get; set; }
        public NotificationType? NotificationType { get; set; }
        public string RelatedEntityType { get; set; }//what is this???
        public int? RelatedEntityId { get; set; }//what is this???  is it fk???
        public bool SentViaEmail { get; set; }
        public bool SentViaSMS { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public ApplicationUser User { get; set; }
    }
}

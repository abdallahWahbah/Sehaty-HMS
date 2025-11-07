using Sehaty.Core.Entites;
using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.NotificationsDTOs
{
    public class AllNotificationsDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; } 
        public NotificationPriority? Priority { get; set; }
        public NotificationType? NotificationType { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public bool SentViaEmail { get; set; }
        public bool SentViaSMS { get; set; }
        public int UserId { get; set; } 
        public string UserName { get; set; }
    }
}

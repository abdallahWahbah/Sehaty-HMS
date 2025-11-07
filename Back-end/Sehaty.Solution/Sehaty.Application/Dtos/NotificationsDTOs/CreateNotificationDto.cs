using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.NotificationsDTOs
{
    public class CreateNotificationDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime? ReadAt { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        [MaxLength(255)]
        public NotificationPriority Priority { get; set; }
        public NotificationType? NotificationType { get; set; }
        [Required]
        [MaxLength(10)]
        public string RelatedEntityType { get; set; }
        [Required]
        [MaxLength(10)]
        public int? RelatedEntityId { get; set; }
        [Required]
        public bool SentViaEmail { get; set; }
        [Required]
        public bool SentViaSMS { get; set; }
        public int UserId { get; set; }
        
    }
}

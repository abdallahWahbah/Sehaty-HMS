using Sehaty.Core.Entities.User_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class Notification : BaseEntity
    {
		public string Title { get; set; }
		public string Message { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ReadAt { get; set; }//how we would track if notification is read or not????
		public bool IsRead { get; set; } //how we would track if notification is read or not????
		public string Priority { get; set; }
		public NotificationType NotificationType { get; set; }
		public string? RelatedEntityType { get; set; }
		public int RelatedEntityId { get; set; }
		public bool SentViaEmail { get; set; }
		public bool SentViaSMS { get; set; }
		public int UserId { get; set; } // Foreign key to User
		public ApplicationUser User { get; set; }
	}
}

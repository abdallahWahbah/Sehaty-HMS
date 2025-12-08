namespace Sehaty.Core.Entities.Business_Entities.Appointments
{
    public enum AuditAction
    {
        Created,
        Rescheduled,
        Canceled,
        CheckedIn,
        NoShow,
        Completed
    }
    public enum ChangedByRole
    {
        Patient,
        Reception,
        Admin,
    }
    public class AppointmentAuditLog : BaseEntity
    {
        public int AppointmentId { get; set; }
        public AuditAction Action { get; set; }
        public DateTime OldDate { get; set; }
        public DateTime NewDate { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public ChangedByRole ChangedBy { get; set; }
    }
}

namespace Sehaty.Core.Entities.User_Entities
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public string CodeHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int Attempts { get; set; } = 0;
        public string RequestedFromIp { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

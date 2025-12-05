namespace Sehaty.Core.Entities.Business_Entities
{
    public enum WalletTransactionType
    {
        Credit = 1,
        Debit = 2
    }

    public class WalletTransaction : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }

        public string Reference { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

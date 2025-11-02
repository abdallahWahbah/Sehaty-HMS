namespace Sehaty.Core.Entites
{
    public enum BillingStatus
    {
        Pending,
        Paid,
        Partially,
        Refunded,
        Canceled
    }
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        MobileWallet,
        BankTransfer
    }
    public class Billing : BaseEntity
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime BillingDate { get; set; }
        public decimal Amount { get; set; }
        public BillingStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
    }
}

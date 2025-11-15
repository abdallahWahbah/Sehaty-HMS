using Sehaty.Core.Entities.Business_Entities.Appointments;

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
        public DateTime BillDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public BillingStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string ItemsDetail { get; set; }
        public string TransactionId { get; set; }
        public decimal? CommissionApplied { get; set; }
        public decimal? NetAmount { get; set; }
        public string Notes { get; set; }

        // Navigation
        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
    }
}

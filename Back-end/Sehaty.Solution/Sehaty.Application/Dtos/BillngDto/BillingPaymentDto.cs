using Sehaty.Core.Entites;

namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingPaymentDto
    {
        public int BillingId { get; set; }
        public decimal PaidAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionId { get; set; }

    }
}

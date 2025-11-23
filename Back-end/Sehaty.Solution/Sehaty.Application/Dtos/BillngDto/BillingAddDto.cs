
namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingAddDto
    {
        public int AppointmentId { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public int TotalAmount { get; set; }

    }
}

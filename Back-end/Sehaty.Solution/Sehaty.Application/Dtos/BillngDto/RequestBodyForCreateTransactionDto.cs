namespace Sehaty.Application.Dtos.BillngDto
{
    public class RequestBodyForCreateTransactionDto
    {
        public int OrderId { get; set; }
        public string PaymentKey { get; set; }
        public string RedirectUrl { get; set; }


    }
}

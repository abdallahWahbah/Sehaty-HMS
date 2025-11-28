namespace Sehaty.Application.Dtos.BillngDto
{


    public enum PaymentProvider
    {
        None = 0,
        PaymobEgy1 = 1,
        PaymobEgy2 = 2
    }

    public class PaymentSettings
    {
        public int PaymentProvider { get; set; }
        public bool AcceptOnlinePayments { get; set; }
    }

    public class PaymobEgy2Settings
    {
        public string PublicKey { get; set; }
        public string SKey { get; set; }
        public int CardIntegrationId { get; set; }
        public int WalletIntegrationId { get; set; }
        public string AccountHMAC { get; set; }
    }
    public class ResponseOrderCreation
    {
        public string client_secret { get; set; }
    }


    public class PaymobCallbackPostModel
    {
        public string type { get; set; }
        public TransactionObj obj { get; set; }
    }

    public class TransactionObj
    {
        public int id { get; set; }
        public bool success { get; set; }
        public int amount_cents { get; set; }
        public string currency { get; set; }
        public OrderInfo order { get; set; }
        public TransactionData data { get; set; }
    }

    public class OrderInfo
    {
        public int id { get; set; }
        public string merchant_order_id { get; set; }
    }

    public class TransactionData
    {
        public string message { get; set; }
    }

}

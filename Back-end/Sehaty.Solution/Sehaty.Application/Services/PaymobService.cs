namespace Sehaty.Application.Services
{
    public class PaymobService : IPaymobService
    {

        private readonly PaymobEgy2Settings settings;
        private readonly string apiSecretKey;


        public PaymobService(IOptions<PaymobEgy2Settings> paymobSettings)
        {
            settings = paymobSettings.Value;
            apiSecretKey = settings.SKey ?? Environment.GetEnvironmentVariable("PaymobSKey");
        }

        public async Task<(string, int)> GetPaymentLinkAsync(int appointmentId, int totalAmount)
        {
            var (clientSecret, orderId) = await CreateIntentionRequest(appointmentId, totalAmount);

            if (string.IsNullOrEmpty(clientSecret))
                return (null, 0);

            string url = $"https://accept.paymob.com/unifiedcheckout/?publicKey={settings.PublicKey}&clientSecret={clientSecret}";
            return (url, orderId);
        }

        public bool ValidateHMAC(string dataString, string expectedHmac)
        {
            if (string.IsNullOrEmpty(settings.AccountHMAC) ||
                string.IsNullOrEmpty(dataString) ||
                string.IsNullOrEmpty(expectedHmac))
                return false;

            var computedHmac = GenerateHmacSHA512(settings.AccountHMAC, dataString);
            return string.Equals(computedHmac, expectedHmac, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<(string, int)> CreateIntentionRequest(int appointmentId, int totalAmount)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("Authorization", $"Token {apiSecretKey}");

                int amountInCents = totalAmount * 100;

                var body = new
                {
                    amount = amountInCents,
                    currency = "EGP",
                    special_reference = Guid.NewGuid().ToString(),
                    payment_methods = new[]
                    {
                settings.CardIntegrationId,
                settings.WalletIntegrationId
                    },
                    items = Array.Empty<object>(),
                    billing_data = new
                    {
                        first_name = "Customer",
                        last_name = "Name",
                        email = "customer@example.com",
                        phone_number = "+201000000000",
                        country = "EG"
                    }
                };

                var response = await client.PostAsJsonAsync(
                    "https://accept.paymob.com/v1/intention/",
                    body
                );

                var content = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode)
                {
                    return (null, 0);
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseOrderCreation>();
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;
                int orderId = root.GetProperty("intention_order_id").GetInt32();

                return (result?.client_secret, orderId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return (null, 0);
            }
        }
        private static string GenerateHmacSHA512(string key, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<bool> RefundPaymentAsync(string transactionId, decimal amountToRefund)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("Authorization", $"Token {apiSecretKey}");

                int amountInCents = (int)(amountToRefund * 100);

                var body = new
                {
                    transaction_id = transactionId,
                    amount_cents = amountInCents
                };

                string apiUrl = "https://accept.paymob.com/api/acceptance/void_refund/refund";

                Console.WriteLine($"Sending Refund Request:");
                Console.WriteLine($"Transaction ID: {transactionId}");
                Console.WriteLine($"Amount: {amountToRefund} EGP ({amountInCents} cents)");

                var response = await client.PostAsJsonAsync(apiUrl, body);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Refund Successful: {content}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Refund Failed ({response.StatusCode}): {content}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refund Exception: {ex.Message}");
                return false;
            }
        }
    }

}
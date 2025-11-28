using System.Net.Http.Json;

namespace Sehaty.Application.Services
{
    public class BillingService : IBillingService
    {

        private readonly PaymobEgy2Settings _settings;

        public BillingService(IUnitOfWork _unit, IOptions<PaymobEgy2Settings> paymobSettings)
        {
            _settings = paymobSettings.Value;
        }

        public async Task<string> GetPaymentLinkAsync(int appointmentId, int totalAmount)
        {
            string clientSecret = await CreateIntentionRequest(appointmentId, totalAmount);

            if (string.IsNullOrEmpty(clientSecret))
                return null;

            string url = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_settings.PublicKey}&clientSecret={clientSecret}";
            var billing = new Billing
            {
                AppointmentId = appointmentId,
                TotalAmount = totalAmount / 100m,
                BillDate = DateTime.UtcNow,
                Status = BillingStatus.Pending
            };
            return url;
        }

        public bool ValidateHMAC(string dataString, string expectedHmac)
        {
            if (string.IsNullOrEmpty(_settings.AccountHMAC) ||
                string.IsNullOrEmpty(dataString) ||
                string.IsNullOrEmpty(expectedHmac))
                return false;

            var computedHmac = GenerateHmacSHA512(_settings.AccountHMAC, dataString);
            return string.Equals(computedHmac, expectedHmac, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> CreateIntentionRequest(int appointmentId, int totalAmount)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("Authorization", $"Token {_settings.SKey}");

                int amountInCents = totalAmount * 100;

                var body = new
                {
                    amount = amountInCents,
                    currency = "EGP",
                    special_reference = appointmentId.ToString(),
                    payment_methods = new[]
                    {
                _settings.CardIntegrationId,
                _settings.WalletIntegrationId
            },
                    items = new object[] { },
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
                    Console.WriteLine($"Status: {response.StatusCode}");
                    Console.WriteLine($"Error: {content}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseOrderCreation>();
                return result?.client_secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
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
                client.DefaultRequestHeaders.Add("Authorization", $"Token {_settings.SKey}");

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
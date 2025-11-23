using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.Specifications.PatientSpec;
using System.Net.Http.Json;

namespace Sehaty.Application.Services
{
    public class BillingService(HttpClient _http, IConfiguration config, IUnitOfWork unit, IMapper mapper) : IBillingService
    {

        public async Task<RequestBodyForCreateTransactionDto> CreateEscrowTransactionAsync(int appointmentId, int patientId, int totalAmount)
        {


            var token = await AuthenticateAsync();

            var orderId = await CreateOrderAsync(token, totalAmount);

            var paymentKey = await CreatePaymentKeyAsync(patientId, token, orderId, totalAmount);

            var redirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/980937?payment_token={paymentKey}";

            var response = new RequestBodyForCreateTransactionDto
            {
                OrderId = orderId,
                PaymentKey = paymentKey,
                RedirectUrl = redirectUrl
            };
            return response;
        }


        public async Task<string> AuthenticateAsync()
        {
            var _apiKey = config["PayMob:api_key"];
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("PayMob API Key is not configured");

            }

            var body = new { api_key = _apiKey };

            var response = await _http.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", body);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return result["token"].ToString();
        }


        public async Task<int> CreateOrderAsync(string token, int amount)
        {
            var body = new
            {
                auth_token = token,
                amount_cents = amount * 100,
                currency = "EGP",
            };

            var response = await _http.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", body);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return int.Parse(result["id"].ToString());
        }


        public async Task<string> CreatePaymentKeyAsync(int patientId, string token, int orderId, int amount)
        {
            var spec = new PatientSpecifications(b => b.Id == patientId);
            var patientData = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            if (patientData is null) throw new Exception("Billing information not found for the patient");

            var billingData = new
            {
                email = patientData.User.Email,
                first_name = patientData.User.FirstName,
                last_name = patientData.User.LastName,
                phone_number = patientData.User.PhoneNumber,
                country = "EG",
                street = "NA",
                building = "NA",
                floor = "NA",
                apartment = "NA",
                city = "NA"

            };

            var body = new
            {
                auth_token = token,
                amount_cents = amount * 100,
                expiration = 3600,
                order_id = orderId,
                billing_data = billingData,
                currency = "EGP",
                integration_id = 5404105,
                lock_order_when_paid = true,

            };

            var response = await _http.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", body);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return result["token"].ToString();
        }

        public async Task<string> GetPaymentStatusAsync(int orderId)
        {
            var authToken = await AuthenticateAsync();

            var url = $"https://accept.paymob.com/api/ecommerce/orders/{orderId}";

            using var client = new HttpClient();

            var requestBody = new
            {
                auth_token = authToken
            };

            var response = await client.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error getting payment status: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            if (result.ContainsKey("payment_status"))
            {
                return result["payment_status"].ToString();
            }

            return "Unknown";

        }
        public async Task<(Billing, string)> AuthorizeEscrowAsync(BillingAddDto model)
        {
            var spec = new AppointmentSpecifications(a => a.Id == model.AppointmentId);
            var appointmentData = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);
            var data = await CreateEscrowTransactionAsync(model.AppointmentId, appointmentData.Patient.Id, model.TotalAmount);

            var billing = mapper.Map<Billing>(model);

            billing.PatientId = appointmentData.Patient.Id;
            billing.TransactionId = data.OrderId.ToString();

            await unit.Repository<Billing>().AddAsync(billing);
            await unit.CommitAsync();
            return (billing, data.RedirectUrl);
        }










    }
}

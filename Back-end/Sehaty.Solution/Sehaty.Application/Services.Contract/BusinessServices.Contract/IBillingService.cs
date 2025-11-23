
namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IBillingService
    {
        public Task<RequestBodyForCreateTransactionDto> CreateEscrowTransactionAsync(int appointmentId, int patientId, int totalAmount);
        public Task<string> AuthenticateAsync();
        public Task<int> CreateOrderAsync(string token, int amount);
        public Task<string> CreatePaymentKeyAsync(int patientId, string token, int orderId, int amount);
        public Task<string> GetPaymentStatusAsync(int orderId);
        public Task<(Billing, string)> AuthorizeEscrowAsync(BillingAddDto model);

    }
}

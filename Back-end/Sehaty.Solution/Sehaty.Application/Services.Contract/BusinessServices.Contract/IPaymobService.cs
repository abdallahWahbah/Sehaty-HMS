namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPaymobService
    {
        Task<(string, int)> GetPaymentLinkAsync(int appointmentId, int totalAmount);
        bool ValidateHMAC(string dataString, string expectedHmac);
        Task<bool> RefundPaymentAsync(string transactionId, decimal amountToRefund);

    }

}

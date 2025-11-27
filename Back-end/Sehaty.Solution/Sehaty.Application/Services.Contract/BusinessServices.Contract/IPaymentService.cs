namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPaymentService
    {
        Task<(string? link, int? billingId)> GetPaymentLinkAsync(int appointmentId, int totalAmount);
        Task<bool> ProcessRefundAsync(int billingId, decimal? partialAmount = null);
    }
}

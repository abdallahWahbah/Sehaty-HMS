using Microsoft.AspNetCore.Mvc;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPaymentService
    {
        Task<Result<(string link, int? billingId)>> GetPaymentLinkAsync(int appointmentId,int totalAmount);
        Task<bool> ProcessRefundAsync(int billingId,decimal? partialAmount = null);
        Task<Result<ContentResult>> CheckSuccessAsync(int id,bool success);
        Task<Result<string>> CallbackAsync(PaymobCallbackPostModel model);
        //Task<Result> CancelConfirmedAppointmentByDoctor(int appointmentId);
    }
}

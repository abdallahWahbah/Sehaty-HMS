namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IBillingService
    {
        public Task<string> CreateEscrowTransactionAsync();
    }
}

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface ISmsSender
    {
        MessageResource SendSmsAsync(string phoneNumber, string message);

    }
}

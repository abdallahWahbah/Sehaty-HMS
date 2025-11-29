namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface ISmsSender
    {
        MessageResource SendSms(string phoneNumber, string message);

    }
}

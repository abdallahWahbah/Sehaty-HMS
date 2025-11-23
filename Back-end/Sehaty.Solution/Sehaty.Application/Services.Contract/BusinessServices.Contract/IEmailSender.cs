namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}

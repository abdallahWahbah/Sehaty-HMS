using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;

namespace Sehaty.Infrastructure.Service.Email
{
    public class EmailSender(IConfiguration config) : IEmailSender
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = config["EmailSettings:SmtpHost"];
            var port = int.Parse(config["EmailSettings:SmtpPort"]);
            var from = config["EmailSettings:FromEmail"];
            var password = config["EmailSettings:Password"];
            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(from, password)
            };
            var message = new MailMessage(from, to, subject, body)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(message);
        }
    }
}

using Microsoft.Extensions.Options;
using Sehaty.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Sehaty.Infrastructure.Service.SMS
{
    public class SmsSender : ISmsSender
    {
        private readonly TwilioSettings _settings;

        public SmsSender(IOptions<TwilioSettings> settings)
        {
            _settings = settings.Value;
            Console.WriteLine("Twilio Account SID: " + _settings.AccountSID);
            Console.WriteLine("Twilio Auth Token: " + _settings.AuthToken);
            Console.WriteLine("Twilio From Phone: " + _settings.TwilioPhoneNumber);
        }


        public MessageResource SendSmsAsync(string phoneNumber, string message)
        {
            TwilioClient.Init(_settings.AccountSID, _settings.AuthToken);
            var result = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_settings.TwilioPhoneNumber),
                to: phoneNumber
                );
            return result;
        }
    }
}

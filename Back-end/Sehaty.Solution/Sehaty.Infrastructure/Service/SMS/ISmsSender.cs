using Sehaty.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Sehaty.Infrastructure.Service.SMS
{
    public interface ISmsSender
    {
        MessageResource SendSmsAsync(string phoneNumber, string message);

    }
}

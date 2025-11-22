using Sehaty.Application.Services.Contract.BusinessServices.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services
{
    public class BillingService : IBillingService
    {
        public Task<string> CreateEscrowTransactionAsync()
        {
            throw new NotImplementedException();
        }
    }
}

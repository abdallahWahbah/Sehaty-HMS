using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IBillingService
    {
        public Task<string> CreateEscrowTransactionAsync();
    }
}

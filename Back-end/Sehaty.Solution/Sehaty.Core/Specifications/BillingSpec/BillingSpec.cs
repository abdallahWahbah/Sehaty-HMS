using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specefications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.BillingSpec
{
    public class BillingSpec : BaseSpecefication<Billing>
    {
        public BillingSpec()
        {
            AddIncludes();
        }
        public BillingSpec(int id) : base(P => P.Id == id)
        {
            AddIncludes();
        }
        public BillingSpec(Expression<Func<Billing, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }



        void AddIncludes()
        {
            Includes.Add(P => P.Appointment);
            Includes.Add(P => P.Patient);
        }
    }
}

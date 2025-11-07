using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specefications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.Notifications_Specs
{
    public class NotificationSpecifications: BaseSpecefication<Notification>
    {
        public NotificationSpecifications()
        {
            AddIncludes();
        }
        public NotificationSpecifications(int id):base(N=>N.Id==id)
        {
            AddIncludes();
        }
        public NotificationSpecifications(Expression<Func<Notification, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }
        void AddIncludes()
        {
            Includes.Add(P => P.User);
           
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specefications;

namespace Sehaty.Core.Specifications.Appointment_Specs
{
    public class AppointmentSpecifications : BaseSpecefication<Appointment>
    {
        public AppointmentSpecifications()
        {
            AddIncludes();
        }
        public AppointmentSpecifications(int id) : base(P => P.Id == id)
        {
            AddIncludes();
        }
        public AppointmentSpecifications(Expression<Func<Appointment, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }



        void AddIncludes()
        {
            Includes.Add(P => P.Patient);
            Includes.Add(P => P.Doctor);
        }
    }
}

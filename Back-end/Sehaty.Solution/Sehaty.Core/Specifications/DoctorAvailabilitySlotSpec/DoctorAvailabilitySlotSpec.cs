using Sehaty.Core.Entites;
using Sehaty.Core.Specefications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.DoctorAvailabilitySlotSpec
{
    public class DoctorAvailabilitySlotSpec : BaseSpecefication<DoctorAvailabilitySlot>
    {
        public DoctorAvailabilitySlotSpec()
        {
            AddIncludes();
        }

        public DoctorAvailabilitySlotSpec(Expression<Func<DoctorAvailabilitySlot, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(slot => slot.Doctor);
        }
    }
}

using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.DoctorAvailabilitySlotSpec
{
    public class DoctorAppointmentSlotspec : BaseSpecefication<DoctorAppointmentSlot>
    {
        public DoctorAppointmentSlotspec()
        {
            AddIncludes();
        }

        public DoctorAppointmentSlotspec(Expression<Func<DoctorAppointmentSlot, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(slot => slot.Appointment);
        }
    }
}

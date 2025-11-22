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

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

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
            IncludeStrings.Add("Patient.User");
        }
    }
}

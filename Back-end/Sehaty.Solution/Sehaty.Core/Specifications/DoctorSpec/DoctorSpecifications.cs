namespace Sehaty.Core.Specifications.DoctorSpec
{
    public class DoctorSpecifications : BaseSpecefication<Doctor>
    {
        public DoctorSpecifications()
        {
            AddIncludes();
        }
        public DoctorSpecifications(int id) : base(D => D.Id == id)
        {
            AddIncludes();
        }
        public DoctorSpecifications(Expression<Func<Doctor, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        void AddIncludes()
        {
            Includes.Add(D => D.User);
            Includes.Add(D => D.Department);
        }
        void AddIncludesWithLists()
        {
            Includes.Add(D => D.Appointments);
            Includes.Add(D => D.Prescriptions);
            Includes.Add(D => D.User);
            Includes.Add(D => D.Department);
        }
    }
}

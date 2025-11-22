namespace Sehaty.Core.Specifications.DepartmentSpec
{
    public class DepartmentSpecifications : BaseSpecefication<Department>
    {
        public DepartmentSpecifications()
        {
            AddIncludes();
        }
        public DepartmentSpecifications(int id) : base(D => D.Id == id)
        {
            AddIncludes();
        }

        void AddIncludes()
        {
            Includes.Add(D => D.Doctors);
        }
    }
}

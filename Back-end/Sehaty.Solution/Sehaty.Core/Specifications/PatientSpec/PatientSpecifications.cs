using Sehaty.Core.Entites;
using Sehaty.Core.Specefications;
using System.Linq.Expressions;

namespace Sehaty.Core.Specifications.PatientSpec
{
    public class PatientSpecifications : BaseSpecefication<Patient>
    {
        public PatientSpecifications()
        {
            AddIncludes();
        }
        public PatientSpecifications(int id) : base(D => D.Id == id)
        {
            AddIncludes();
        }
        public PatientSpecifications(Expression<Func<Patient, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        void AddIncludes()
        {
            Includes.Add(P => P.User);
        }
        void AddIncludesWithLists()
        {
            Includes.Add(P => P.Appointments);
            Includes.Add(P => P.Prescriptions);
            Includes.Add(P => P.User);
            Includes.Add(P => P.Billings);
        }
    }
}

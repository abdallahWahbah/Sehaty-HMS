using Sehaty.Core.Entites;
using Sehaty.Core.Specefications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.MedicalReord
{
    public class MedicalRecordSpec :BaseSpecefication<MedicalRecord>
    {
        public MedicalRecordSpec()
        {
            AddIncludes();
        }

        public MedicalRecordSpec(Expression<Func<MedicalRecord, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(m=>m.Appointment);
            Includes.Add(m => m.Prescriptions);
        }
    }
}

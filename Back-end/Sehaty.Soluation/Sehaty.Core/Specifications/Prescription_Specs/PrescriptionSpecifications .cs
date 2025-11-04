using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specefications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Specifications.Prescription_Specs
{
    public class PrescriptionSpecifications : BaseSpecefication<Prescription>
    {

        public PrescriptionSpecifications()
        {
            AddIncludes();
        }
        public PrescriptionSpecifications(int id):base(P=>P.Id==id)
        {
            AddIncludes();
        }  
        public PrescriptionSpecifications(Expression<Func<Prescription,bool>> criteria):base(criteria)
        {
            AddIncludes();
        }



        void AddIncludes()
        {
            Includes.Add(P => P.Appointment);
            Includes.Add(P => P.Patient);
            Includes.Add(P => P.Doctor);
            Includes.Add(P => P.MedicalRecord);
        }
    }
}

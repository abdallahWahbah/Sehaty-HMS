namespace Sehaty.Core.Specifications.Prescription_Specs
{
    public class PrescriptionSpecifications : BaseSpecefication<Prescription>
    {

        public PrescriptionSpecifications()
        {
            AddIncludes();
        }
        public PrescriptionSpecifications(int id) : base(P => P.Id == id)
        {
            AddIncludes();
        }
        public PrescriptionSpecifications(Expression<Func<Prescription, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }



        void AddIncludes()
        {
            Includes.Add(P => P.Appointment);
            Includes.Add(P => P.Patient);
            Includes.Add(P => P.Doctor);
            Includes.Add(P => P.MedicalRecord);
            Includes.Add(P => P.Medications);
            IncludeStrings.Add("Medications.Medication");

        }
    }
}

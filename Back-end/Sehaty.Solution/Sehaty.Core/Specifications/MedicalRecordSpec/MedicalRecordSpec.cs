namespace Sehaty.Core.Specifications.MedicalRecordSpec
{
    public class MedicalRecordSpec : BaseSpecefication<MedicalRecord>
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
            Includes.Add(m => m.Patient);
            Includes.Add(m => m.Prescriptions);
        }
    }
}

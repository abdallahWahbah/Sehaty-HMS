namespace Sehaty.Core.Specifications.MedicalReord
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
            Includes.Add(m => m.Appointment);
            Includes.Add(m => m.Prescriptions);
        }
    }
}

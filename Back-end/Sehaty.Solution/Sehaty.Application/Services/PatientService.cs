namespace Sehaty.Application.Services
{
    public class PatientService(IMapper mapper, IUnitOfWork unit) : IPatientService
    {
        public async Task<Patient> AddPatientAsync(PatientAddDto dto)
        {


            var patientToAdd = mapper.Map<Patient>(dto);


            patientToAdd.Patient_Id = await GeneratePatientIdAsync();

            await unit.Repository<Patient>().AddAsync(patientToAdd);
            await unit.CommitAsync();

            return patientToAdd;


        }
        private async Task<string> GeneratePatientIdAsync()
        {
            var currentYear = DateTime.Now.Year;
            var prefix = $"PT-{currentYear}-";


            var lastPatient = await unit.Repository<Patient>()
                .FindBy(p => p.Patient_Id.StartsWith(prefix))
                .OrderByDescending(p => p.Patient_Id)
                .FirstOrDefaultAsync();

            int sequence = 1;

            if (lastPatient != null)
            {
                var lastSeq = lastPatient.Patient_Id.Split('-').Last();
                sequence = int.Parse(lastSeq) + 1;
            }

            return $"{prefix}{sequence.ToString().PadLeft(4, '0')}";
        }

    }
}

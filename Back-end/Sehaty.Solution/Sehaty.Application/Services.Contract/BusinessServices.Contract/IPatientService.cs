namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPatientService
    {
        Task<Patient> AddPatientAsync(PatientAddDto dto);
        //Task<Result<GetPatientDto>> RegisterPatientAsync(RegisterPatientDto dto);

    }
}

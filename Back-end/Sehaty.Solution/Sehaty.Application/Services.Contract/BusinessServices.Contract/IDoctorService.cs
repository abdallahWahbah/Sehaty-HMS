namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IDoctorService
    {
        Task<Doctor> AddDoctorAsync(DoctorAddUpdateDto dto);
        Task<Doctor> UpdateDoctorAsync(int id, DoctorAddUpdateDto dto);
        Task<bool> DeleteDoctorAsync(int id);
    }
}

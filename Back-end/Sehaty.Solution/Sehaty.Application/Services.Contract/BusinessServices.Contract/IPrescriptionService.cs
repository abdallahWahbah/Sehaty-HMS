namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPrescriptionService
    {
        public Task<Result<Prescription>> CreatePrescriptionAsync(CreatePrescriptionsDto dto);
        public Task<Result<Prescription>> GetPrescriptionDetailsAsync(int id);
        public Task<Result> DeletePrescriptionAsync(int id);
        public Task<Result> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto dto);
        public Task<Result<IEnumerable<Prescription>>> GetPatientPrescriptionsAsync(int patientId);
        public Task<Result<byte[]>> GetPrescriptionPdfFile(int id);
    }
}

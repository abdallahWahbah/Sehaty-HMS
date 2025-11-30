namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPrescriptionService
    {
        public Task<Prescription> CreatePrescriptionAsync(CreatePrescriptionsDto dto);
        public Task<Prescription> GetPrescriptionDetailsAsync(int id);
        public Task DeletePrescriptionAsync(int id);
        public Task UpdatePrescriptionAsync(int id, UpdatePrescriptionDto dto);
        public Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(int patientId);
        byte[] GeneratePrescriptionPdf(Prescription prescription);
    }
}

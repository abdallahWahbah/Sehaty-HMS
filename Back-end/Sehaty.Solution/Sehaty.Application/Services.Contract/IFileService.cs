namespace Sehaty.Application.Services.Contract
{
    public interface IFileService
    {
        Task<string> UploadDoctorImageAsync(IFormFile file);
        void DeleteDoctorImage(string fileName);
    }
}

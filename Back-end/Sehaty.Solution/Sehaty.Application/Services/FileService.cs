namespace Sehaty.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private string DoctorsPath => Path.Combine(_env.WebRootPath, "images", "doctors");

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadDoctorImageAsync(IFormFile file)
        {
            if (!Directory.Exists(DoctorsPath))
                Directory.CreateDirectory(DoctorsPath);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(DoctorsPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }


        public void DeleteDoctorImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            string fullPath = Path.Combine(DoctorsPath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

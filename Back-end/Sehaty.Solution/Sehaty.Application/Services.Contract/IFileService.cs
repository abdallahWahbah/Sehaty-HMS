using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.Contract
{
    public interface IFileService
    {
        Task<string> UploadDoctorImageAsync(IFormFile file);
        void DeleteDoctorImage(string fileName);
    }
}

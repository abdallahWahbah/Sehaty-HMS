using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IDoctorService
    {
        Task<Doctor> AddDoctorAsync(DoctorAddUpdateDto dto);
        Task<Doctor> UpdateDoctorAsync(int id, DoctorAddUpdateDto dto);
        Task<bool> DeleteDoctorAsync(int id);
    }
}

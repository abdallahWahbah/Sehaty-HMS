using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QuestPDF.Infrastructure;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Application.Services.Contract;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.DoctorService
{
    public class DoctorService(IUnitOfWork unit, IMapper mapper, IFileService fileService) : IDoctorService
    {
        public async Task<Doctor> AddDoctorAsync(DoctorAddUpdateDto dto)
        {
            var doctor = mapper.Map<Doctor>(dto);
            string uploadedImage = null;

            try
            {
                // Upload image if exists
                if (dto.ProfilePhoto != null)
                {
                    uploadedImage = await fileService.UploadDoctorImageAsync(dto.ProfilePhoto);
                    doctor.ProfilePhoto = uploadedImage;
                }

                await unit.Repository<Doctor>().AddAsync(doctor);
                await unit.CommitAsync();

                return doctor;
            }
            catch (Exception)
            {
                // Delete uploaded file if DB failed
                if (uploadedImage != null)
                    fileService.DeleteDoctorImage(uploadedImage);

                throw;
            }
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                return false;

            string oldImage = doctor.ProfilePhoto;


            unit.Repository<Doctor>().Delete(doctor);
            int success = await unit.CommitAsync();


            if (!string.IsNullOrEmpty(oldImage) && success > 0)
                fileService.DeleteDoctorImage(oldImage);

            return true;
        }

        public async Task<Doctor> UpdateDoctorAsync(int id, DoctorAddUpdateDto dto)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                return null;

            mapper.Map(dto, doctor);

            string oldImage = doctor.ProfilePhoto;
            string newUploaded = null;

            try
            {

                if (dto.ProfilePhoto != null)
                {
                    newUploaded = await fileService.UploadDoctorImageAsync(dto.ProfilePhoto);
                    doctor.ProfilePhoto = newUploaded;
                }


                await unit.CommitAsync();

                // حذف القديمة لو تم رفع صورة جديدة
                if (newUploaded != null && !string.IsNullOrEmpty(oldImage))
                {
                    fileService.DeleteDoctorImage(oldImage);
                }

                return doctor;
            }
            catch (Exception)
            {
                // امسح الصورة لو حصل اى ايرور بعد الرفع
                if (newUploaded != null)
                    fileService.DeleteDoctorImage(newUploaded);

                throw;
            }
        }

    }
}

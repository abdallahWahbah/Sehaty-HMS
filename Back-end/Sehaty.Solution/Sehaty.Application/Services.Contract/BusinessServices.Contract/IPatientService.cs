using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.PatientDto;
using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPatientService
    {
        Task<Patient> AddPatientAsync(PatientAddDto dto);
    }
}

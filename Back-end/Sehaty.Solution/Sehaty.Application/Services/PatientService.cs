using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Sehaty.Application.Dtos.PatientDto;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Services
{
    public class PatientService(IMapper mapper, IUnitOfWork unit) : IPatientService
    {
        public async Task<Patient> AddPatientAsync(PatientAddDto dto)
        {


            var patientToAdd = mapper.Map<Patient>(dto);


            patientToAdd.PatientId = await GeneratePatientIdAsync();

            await unit.Repository<Patient>().AddAsync(patientToAdd);
            await unit.CommitAsync();

            return patientToAdd;


        }
        private async Task<string> GeneratePatientIdAsync()
        {
            var currentYear = DateTime.Now.Year;
            var prefix = $"PT-{currentYear}-";


            var lastPatient = await unit.Repository<Patient>()
                .FindByAsync(p => p.PatientId.StartsWith(prefix))
                .OrderByDescending(p => p.PatientId)
                .FirstOrDefaultAsync();

            int sequence = 1;

            if (lastPatient != null)
            {
                var lastSeq = lastPatient.PatientId.Split('-').Last();
                sequence = int.Parse(lastSeq) + 1;
            }

            return $"{prefix}{sequence.ToString().PadLeft(4, '0')}";
        }

    }
}

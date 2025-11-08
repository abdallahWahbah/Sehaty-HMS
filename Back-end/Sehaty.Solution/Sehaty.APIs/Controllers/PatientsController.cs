using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.PatientDto;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.PatientSpec;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class PatientsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> GetAllPatients()
        {
            var spec = new PatientSpecifications();
            var patients = await unit.Repository<Patient>().GetAllWithSpecAsync(spec);
            if (patients is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetPatientDto>>(patients));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPatientDto>> GetPatientById(int id)
        {
            var spec = new PatientSpecifications(id);
            var patient = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            if (patient is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetPatientDto>(patient));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var patientToDelete = await unit.Repository<Patient>().GetByIdAsync(id);
            if (patientToDelete is null)
                return NotFound(new ApiResponse(404));
            unit.Repository<Patient>().Delete(patientToDelete);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, [FromBody] PatientAddUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var patientToEdit = await unit.Repository<Patient>().GetByIdAsync(id);
                if (patientToEdit is null)
                    return NotFound(new ApiResponse(404));
                mapper.Map(dto, patientToEdit);
                unit.Repository<Patient>().Update(patientToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] PatientAddUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var patientToAdd = mapper.Map<Patient>(dto);
                await unit.Repository<Patient>().AddAsync(patientToAdd);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetPatientById), new { id = patientToAdd.Id }, mapper.Map<GetPatientDto>(patientToAdd));
            }
            return BadRequest(ModelState);
        }
    }
}

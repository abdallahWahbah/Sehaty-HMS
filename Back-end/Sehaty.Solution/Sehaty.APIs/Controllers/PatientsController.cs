using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("Search")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> SearchForPatient([FromQuery] PatientSpecsParams specParam)
        {
            var spec = new PatientSearchSpecification(specParam);
            var patients = await unit.Repository<Patient>().GetAllWithSpecAsync(spec);
            if (patients is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetPatientDto>>(patients));
        }

        [HttpGet("UpdateStatus/{id}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<GetPatientDto>>> UpdatePatientStatus(int id, PatientStatus status)
        {
            var patient = await unit.Repository<Patient>().GetByIdAsync(id);
            if (patient is null)
                return NotFound(new ApiResponse(404));
            patient.Status = status;
            unit.Repository<Patient>().Update(patient);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<GetPatientDto>> GetPatientById(int id)
        {
            var spec = new PatientSpecifications(id);
            var patient = await unit.Repository<Patient>().GetByIdWithSpecAsync(spec);
            if (patient is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetPatientDto>(patient));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Receptionist")]
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
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult> UpdatePatientByStuff(int id, [FromBody] PatientUpdateByStaffDto dto)
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

        [HttpPut("EditProfile/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> EditPatientProfile(int id, [FromBody] PatientUpdateDto dto)
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
        [Authorize(Roles = "Patient,Receptionist")]
        public async Task<ActionResult> AddPatient([FromBody] PatientAddDto dto)
        {
            if (ModelState.IsValid)
            {
                var patientToAdd = mapper.Map<Patient>(dto);
                //patientToAdd.UserId = int.Parse(User.Claims.FirstOrDefault(C => C.Type == ClaimTypes.NameIdentifier)!.Value);
                await unit.Repository<Patient>().AddAsync(patientToAdd);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetPatientById), new { id = patientToAdd.Id }, mapper.Map<GetPatientDto>(patientToAdd));
            }
            return BadRequest(ModelState);
        }

    }
}

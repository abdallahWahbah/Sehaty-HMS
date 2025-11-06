using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.DoctorSpec;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class DoctorsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDoctorDto>>> GetAllDoctors()
        {
            var spec = new DoctorSpecifications();
            var doctors = await unit.Repository<Doctor>().GetAllWithSpecAsync(spec);
            if (doctors is null)
                return NotFound();
            return Ok(mapper.Map<IEnumerable<GetDoctorDto>>(doctors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetDoctorDto>> GetDoctorById(int id)
        {
            var spec = new DoctorSpecifications(id);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(spec);
            if (doctor is null)
                return NotFound();
            return Ok(mapper.Map<GetDoctorDto>(doctor));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var doctorToDelete = await unit.Repository<Doctor>().GetByIdAsync(id);
            if (doctorToDelete is null)
                return NotFound();
            unit.Repository<Doctor>().Delete(doctorToDelete);
            await unit.CommitAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDoctor(int id, [FromBody] DoctorAddUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var doctorToEdit = await unit.Repository<Doctor>().GetByIdAsync(id);
                if (doctorToEdit is null)
                    return NotFound();
                mapper.Map(dto, doctorToEdit);
                unit.Repository<Doctor>().Update(doctorToEdit);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<ActionResult> AddDoctor([FromBody] DoctorAddUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var doctorToAdd = mapper.Map<Doctor>(dto);
                await unit.Repository<Doctor>().AddAsync(doctorToAdd);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetDoctorById), new { id = doctorToAdd.Id }, mapper.Map<GetDoctorDto>(doctorToAdd));
            }
            return BadRequest(ModelState);
        }
    }
}

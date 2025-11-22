using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.DoctorSpec;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class DoctorsController(IUnitOfWork unit, IMapper mapper, IDoctorService doctorService) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDoctorDto>>> GetAllDoctors()
        {
            var spec = new DoctorSpecifications();
            var doctors = await unit.Repository<Doctor>().GetAllWithSpecAsync(spec);
            if (doctors is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<GetDoctorDto>>(doctors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetDoctorDto>> GetDoctorById(int id)
        {
            var spec = new DoctorSpecifications(id);
            int s = 10 / id;
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(spec);
            if (doctor is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<GetDoctorDto>(doctor));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var success = await doctorService.DeleteDoctorAsync(id);
            if (!success)
                return NotFound(new ApiResponse(404));
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDoctor(int id, [FromForm] DoctorAddUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await doctorService.UpdateDoctorAsync(id, dto);

            if (doctor == null)
                return NotFound(new ApiResponse(404));

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> AddDoctor([FromForm] DoctorAddUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await doctorService.AddDoctorAsync(dto);

            return CreatedAtAction(nameof(GetDoctorById),
                new { id = doctor.Id },
                mapper.Map<GetDoctorDto>(doctor));
        }
    }
}

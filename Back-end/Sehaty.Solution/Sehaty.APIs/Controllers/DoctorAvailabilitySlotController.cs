using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.DoctorAvailabilitySlotDto;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.DoctorAvailabilitySlotSpec;
using Sehaty.Core.Specifications.Prescription_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{
    public class DoctorAvailabilitySlotController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {
        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAllDoctorAvailability()
        {
            var spec = new DoctorAvailabilitySlotSpec();
            var doctorAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetAllWithSpecAsync(spec);
            return Ok(mapper.Map<List<DoctorAvailabilityReadDto>>(doctorAvailability));
        }
        //Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorAvailabilityById(int? id)
        {
            var spec = new DoctorAvailabilitySlotSpec(d => d.Id == id);
            var doctorAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetByIdWithSpecAsync(spec);
            return Ok(mapper.Map<DoctorAvailabilityReadDto>(doctorAvailability));
        }

        [HttpGet("getByName{FullName}")]
        public async Task<IActionResult> GetByDoctorName(string FullName)
        {
            var spec = new DoctorAvailabilitySlotSpec(d =>
            (d.Doctor.FirstName + "" + d.Doctor.LastName).Contains(FullName));
            var doctorAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetByIdWithSpecAsync(spec);

            if (doctorAvailability != null)
                return Ok(mapper.Map<DoctorAvailabilityReadDto>(doctorAvailability));

            return NotFound();
        }

        // PostData
        [HttpPost]
        public async Task<IActionResult> AddDoctorAvailability(DoctorAvailabilityAddOrUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var AddDoctorAvailability = mapper.Map<DoctorAvailabilitySlot>(model);
            await unit.Repository<DoctorAvailabilitySlot>().AddAsync(AddDoctorAvailability);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? CreatedAtAction(nameof(GetDoctorAvailabilityById),
                new { id = AddDoctorAvailability.Id }, mapper.Map<DoctorAvailabilityReadDto>(AddDoctorAvailability))
                : BadRequest();
        }

        //UpdateData
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorAvailability(int? id, [FromBody] DoctorAvailabilityAddOrUpdateDto model)
        {

            if (id is null) return BadRequest();
            if (ModelState.IsValid)
            {
                var updateDoctorAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetByIdAsync(id.Value);
                if (updateDoctorAvailability is null)
                    return NotFound();
                mapper.Map(model, updateDoctorAvailability);
                unit.Repository<DoctorAvailabilitySlot>().Update(updateDoctorAvailability);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        //DeleteData
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorAvailability(int? id)
        {
            if (id is null) return BadRequest();
            var doctorAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetByIdAsync(id.Value);
            if (doctorAvailability is null) return NotFound();
            unit.Repository<DoctorAvailabilitySlot>().Delete(doctorAvailability);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? NoContent() : BadRequest(ModelState);
        }


    }
}

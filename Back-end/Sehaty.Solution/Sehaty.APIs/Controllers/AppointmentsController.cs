using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.AppointmentDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class AppointmentsController(IUnitOfWork unit, IMapper mapper) : ApiBaseController
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAllAppointments()
        {
            var spec = new AppointmentSpecifications();
            var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
            return Ok(mapper.Map<List<AppointmentReadDto>>(appointments));
        }

        [HttpGet("GetAppointmentById{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var specs = new AppointmentSpecifications(D => D.Id == id);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(specs);
            return Ok(mapper.Map<AppointmentReadDto>(appointment));

        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentAddOrUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto.AppointmentDateTime < DateTime.Now)
                return BadRequest("Appointment date cannot be in the past.");
            var appointment = mapper.Map<Appointment>(dto);
            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        // PUT: api/Appointments/5 <<works great>>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAppointment(int? id, [FromBody] AppointmentAddOrUpdateDto dto)
        {
            if (id is null) return BadRequest();
            if (ModelState.IsValid)
            {
                var updateAppointment = await unit.Repository<Appointment>().GetByIdAsync(id.Value);
                if (updateAppointment is null)
                    return NotFound();
                mapper.Map(dto, updateAppointment);
                unit.Repository<Appointment>().Update(updateAppointment);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(int? id)
        {
            if (id is null) return BadRequest();
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id.Value);
            if (appointment is null) return NotFound();
            unit.Repository<Appointment>().Delete(appointment);
            var RowAffected = await unit.CommitAsync();
            return RowAffected > 0 ? NoContent() : BadRequest(ModelState);
        }
    }
}

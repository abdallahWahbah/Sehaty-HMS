using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.AppointmentDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class AppointmentsController : ApiBaseController
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public AppointmentsController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAllAppointments()
        {
            var specs = new AppointmentSpecifications();
            var model = await _unit.Repository<Appointment>().GetAllWithSpecAsync(specs);
            var data = _mapper.Map<List<AppointmentReadDto>>(model);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var specs = new AppointmentSpecifications(D => D.Id == id);
            var model = await _unit.Repository<Appointment>().GetByIdWithSpecAsync(specs);
            var data = _mapper.Map<AppointmentReadDto>(model);
            return Ok(data);

        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.AppointmentDateTime < DateTime.Now)
                return BadRequest("Appointment date cannot be in the past.");

            var appointment = _mapper.Map<Appointment>(dto);

            await _unit.Repository<Appointment>().AddAsync(appointment);
            await _unit.CommitAsync();

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        // PUT: api/Appointments/5 <<works great>>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAppointment(int id, [FromBody] AppointmentUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest("Invalid appointment ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _unit.Repository<Appointment>().GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Appointment with ID {id} not found.");

            _mapper.Map(dto, existing);

            _unit.Repository<Appointment>().Update(existing);
            await _unit.CommitAsync();

            return Ok(existing);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid appointment ID.");

            var appointment = await _unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found.");

            _unit.Repository<Appointment>().Delete(appointment);
            await _unit.CommitAsync();

            return Ok($"Appointment with ID {id} deleted successfully.");
        }
    }
}

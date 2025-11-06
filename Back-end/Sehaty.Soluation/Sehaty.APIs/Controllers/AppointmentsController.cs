using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Core.Entites;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Dtos.AppointmentDtos;

namespace Sehaty.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AppointmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public AppointmentsController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        //GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _unit.Repository<Appointment>().GetAllAsync();

            if (appointments == null || !appointments.Any())
                return NotFound("No appointments found.");

            return Ok(appointments);
        }

        //GET: api/Appointments/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid appointment ID.");

            var appointment = await _unit.Repository<Appointment>().GetByIdAsync(id, asNoTracking: true);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found.");

            return Ok(appointment);
        }





    }
}

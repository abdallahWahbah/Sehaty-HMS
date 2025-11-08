using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetAppointmentById/{id}")]
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
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? NoContent() : BadRequest(ModelState);
        }


        // Change AppointmentStatus To No Show if the currentTime has passed the AppointmentTime
        [Authorize(Roles = "Admin,Reception")]
        [HttpPost("NoShow/{id}")]
        public async Task<IActionResult> MarkedStatusAsNoShow(int id, [FromBody] DateTime currentDateTime)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound();
            if (currentDateTime <= appointment.AppointmentDateTime)
            {
                return BadRequest("Cann't mark as No Show before Appointment time");
            }
            if (appointment.Status == AppointmentStatus.InProgress ||
                appointment.Status == AppointmentStatus.NoShow ||
                appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.Canceled)
            {
                return BadRequest("AppointmentStatus cann't be marked as No Show.");
            }
            appointment.Status = AppointmentStatus.NoShow;
            appointment.NoShowTimestamp = currentDateTime;
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok("Status changed successfully") : BadRequest("Failed");
        }


        // Change the status to InProgress after receptionist checkIn the patient
        [Authorize(Roles = "Admin,Reception")]
        [HttpPost("CheckIn/{id}")]
        public async Task<IActionResult> CheckInAppointment(int id)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound();
            if (appointment.Status != AppointmentStatus.Confirmed)
            {
                return BadRequest("only confirmed Appointments can be checkedIn");
            }
            appointment.Status = AppointmentStatus.InProgress;
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok("Patient checkedIn successfully") : BadRequest("Failed");

        }



        //
        //[Authorize(Roles = "Patient,Reception")]
        //[HttpPost("CancelAppointment/{id}")]
        //public async Task<IActionResult> CancelAppointment(int id, [FromBody] DateTime requestTime)
        //{
        //    var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
        //}
    }
}

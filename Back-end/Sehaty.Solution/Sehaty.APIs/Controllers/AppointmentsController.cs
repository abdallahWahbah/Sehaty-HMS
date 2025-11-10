using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.AppointmentDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.UnitOfWork.Contract;
using System.Security.Claims;

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
            if (appointment is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<AppointmentReadDto>(appointment));

        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentAddOrUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400));
            if (dto.AppointmentDateTime < DateTime.Now)
                return BadRequest(new ApiResponse(400, "Appointment date cannot be in the past"));
            var appointment = mapper.Map<Appointment>(dto);
            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, dto);
        }

        // PUT: api/Appointments/5 <<works great>>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAppointment(int? id, AppointmentAddOrUpdateDto dto)
        {
            if (id is null) return BadRequest(new ApiResponse(400));
            if (ModelState.IsValid)
            {
                var updateAppointment = await unit.Repository<Appointment>().GetByIdAsync(id.Value);
                if (updateAppointment is null)
                    return NotFound(new ApiResponse(404));
                mapper.Map(dto, updateAppointment);
                unit.Repository<Appointment>().Update(updateAppointment);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(new ApiResponse(400));
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(int? id)
        {
            if (id is null) return BadRequest(new ApiResponse(400));
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id.Value);
            if (appointment is null) return NotFound(new ApiResponse(404));
            unit.Repository<Appointment>().Delete(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok(new ApiResponse(200, "Deleted successfully")) : BadRequest(new ApiResponse(404));
        }


        // Change AppointmentStatus To No Show if the currentTime has passed the AppointmentTime
        [Authorize(Roles = "Admin,Reception")]
        [HttpPost("NoShow/{id}")]
        public async Task<IActionResult> MarkedStatusAsNoShow(int id, [FromQuery] DateTime currentDateTime)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));
            if (currentDateTime < appointment.AppointmentDateTime)
            {
                return BadRequest(new ApiResponse(400, "Cann't mark as No Show before Appointment time"));
            }
            if (appointment.Status == AppointmentStatus.InProgress ||
                appointment.Status == AppointmentStatus.NoShow ||
                appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.Canceled)
            {
                return BadRequest(new ApiResponse(400, "AppointmentStatus cann't be marked as No Show."));
            }
            appointment.Status = AppointmentStatus.NoShow;
            appointment.NoShowTimestamp = currentDateTime;
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok(new ApiResponse(200, "Status changed successfully")) : BadRequest(new ApiResponse(400, "Failed"));
        }


        // Change the status to InProgress after receptionist checkIn the patient
        //[Authorize(Roles = "Admin,Reception")]
        [HttpPost("CheckIn/{id}")]
        public async Task<IActionResult> CheckInAppointment(int id)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));
            if (appointment.Status != AppointmentStatus.Confirmed)
            {
                return BadRequest(new ApiResponse(400, "only confirmed Appointments can be checkedIn"));
            }
            appointment.Status = AppointmentStatus.InProgress;
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok(new ApiResponse(200, "Patient checkedIn successfully")) : BadRequest(new ApiResponse(400, "Failed"));

        }

        //Cancel Appointment With Policy
        [Authorize(Roles = "Patient,Reception")]
        [HttpPost("CancelAppointment/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));
            var requestTime = DateTime.UtcNow;
            var timeBeforeCancel = appointment.AppointmentDateTime - requestTime;
            if (timeBeforeCancel >= TimeSpan.FromHours(24) || appointment.Status == AppointmentStatus.Emergency)
            {
                appointment.Status = AppointmentStatus.Canceled;
                unit.Repository<Appointment>().Update(appointment);
                var rowsAffected = await unit.CommitAsync();
                return rowsAffected > 0
                    ? Ok(new ApiResponse(200, "Appointment canceled successfully"))
                    : BadRequest(new ApiResponse(400, "Failed to cancel appointment"));
            }
            else
            {
                return BadRequest(new ApiResponse(400, "Cannot cancel appointment within 24 hours unless marked as Emergency"));
            }

        }
        [Authorize(Roles = "Patient,Reception")]
        //RescheduleAppointment
        [HttpPut("RescheduleAppointment/{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromQuery] DateTime newDate)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));
            var requestTime = DateTime.UtcNow;
            var timeBeforeEdit = appointment.AppointmentDateTime - requestTime;
            if (timeBeforeEdit < TimeSpan.FromHours(24) || appointment.Status != AppointmentStatus.Emergency)
            {
                return BadRequest(new ApiResponse(400, "Cannot reschedule appointment within 24 hours unless marked as Emergency"));

            }
            var oldDateTime = appointment.AppointmentDateTime;
            appointment.AppointmentDateTime = newDate;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Admin";
            var changedBy = userRole switch
            {
                "Patient" => ChangedByRole.Patient,
                "Reception" => ChangedByRole.Reception,
                _ => ChangedByRole.Admin
            };

            var auditLog = new AppointmentAuditLog
            {
                AppointmentId = appointment.Id,
                Action = AuditAction.Rescheduled,
                OldDate = oldDateTime,
                NewDate = newDate,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = changedBy,
            };
            await unit.Repository<AppointmentAuditLog>().AddAsync(auditLog);
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok(new ApiResponse(200, "Appointment rescheduled successfully")) : BadRequest(new ApiResponse(400, "Failed to reschedule appointment"));


        }
    }
}

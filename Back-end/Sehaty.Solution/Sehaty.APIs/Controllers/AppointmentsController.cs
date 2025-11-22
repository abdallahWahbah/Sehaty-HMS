using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;
using Sehaty.APIs.Errors;
using Sehaty.Application.Dtos.AppointmentDTOs;
using Sehaty.Application.Dtos.BillngDto;
using Sehaty.Application.Dtos.NotificationsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities.Appointments;
using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.Specifications.MedicalReord;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Service.Email;
using Sehaty.Infrastructure.Service.SMS;
using System.Security.Claims;
using Twilio.TwiML.Messaging;

namespace Sehaty.APIs.Controllers
{

    public class AppointmentsController(IUnitOfWork unit, IMapper mapper, IEmailSender emailSender, ISmsSender smsSender) : ApiBaseController
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

            //  Check if the doctor already has an appointment at the same time
            var spec = new AppointmentSpecifications(a =>
                a.DoctorId == dto.DoctorId && a.AppointmentDateTime == dto.AppointmentDateTime);
            var existingAppointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
            if (existingAppointments.Any())
                return BadRequest(new ApiResponse(400, "Doctor already has an appointment at this time"));
            var appointment = mapper.Map<Appointment>(dto);
            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();
            var patient = await unit.Repository<Patient>().GetByIdAsync(dto.PatientId);
            if (patient is null)
                return NotFound(new ApiResponse(404, "Patient not found"));

            var specappointemnet = new AppointmentSpecifications(a => a.Id == appointment.Id);
            appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(specappointemnet);
            string message = $"تم حجز موعدك مع الطبيب {appointment.Doctor.FirstName + " " + appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime}";

            //Create a DTO for the notification


            var notificationDto = new CreateNotificationDto
            {
                UserId = dto.PatientId,
                Title = "Appointment Booked",
                Message = message,
                Priority = NotificationPriority.High,
                RelatedEntityType = "Appointment",
                RelatedEntityId = appointment.Id,
                SentViaEmail = false,
                IsRead = false,
                NotificationType = NotificationType.Appointment,
                SentViaSMS = false
            };
            var notification = mapper.Map<Notification>(notificationDto);
            await unit.Repository<Notification>().AddAsync(notification);
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
        [HttpPost("ConfirmAppointment/{id}")]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var spec = new AppointmentSpecifications(a => a.Id == id);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);
            if (appointment == null) return NotFound(new ApiResponse(404));
            if (appointment.Status != AppointmentStatus.Pending) return BadRequest(new ApiResponse(400, "Appointment cannot be confirmed"));
            appointment.Status = AppointmentStatus.Confirmed;
            var rowsAffected = await unit.CommitAsync();

            if (rowsAffected <= 0)
                return BadRequest(new ApiResponse(400, "Failed to confirm appointment"));
            var patient = await unit.Repository<Patient>().GetByIdAsync(appointment.PatientId);
            if (patient != null)
            {
                string message = $"تم تأكيد موعدك مع الطبيب {appointment.Doctor.FirstName} {appointment.Doctor.LastName} بتاريخ {appointment.AppointmentDateTime}";

                var notificationDto = new CreateNotificationDto
                {
                    UserId = appointment.PatientId,
                    Title = "Appointment Confirmed",
                    Message = message,
                    Priority = NotificationPriority.High,
                    RelatedEntityType = "Appointment",
                    RelatedEntityId = appointment.Id,
                    SentViaEmail = false,
                    SentViaSMS = true,
                    NotificationType = NotificationType.Appointment,
                    IsRead = false
                };
                var notification = mapper.Map<Notification>(notificationDto);
                await unit.Repository<Notification>().AddAsync(notification);
                await unit.CommitAsync();
                if (!string.IsNullOrEmpty(patient.User.Email))
                {
                    await emailSender.SendEmailAsync(patient.User.Email, "تم تأكيد موعدك", message);
                    notificationDto.SentViaEmail = false;
                }
                if (!string.IsNullOrEmpty(patient.User.PhoneNumber))
                {
                    smsSender.SendSmsAsync(patient.User.PhoneNumber, message);
                    notificationDto.SentViaSMS = true;
                }
                await unit.CommitAsync();
            }

            return Ok(new ApiResponse(200, "Appointment confirmed successfully"));
        }


    }
}

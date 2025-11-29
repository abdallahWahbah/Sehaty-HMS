namespace Sehaty.APIs.Controllers
{

    public class AppointmentsController(IPaymentService paymentService, IUnitOfWork unit, IMapper mapper, IAppointmentService appointmentService, IEmailSender emailSender, ISmsSender smsSender, IWebHostEnvironment env) : ApiBaseController
    {

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAllAppointments()
        {
            var spec = new AppointmentSpecifications();
            var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
            return Ok(mapper.Map<List<AppointmentReadDto>>(appointments));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAllActiveAppointments()
        {
            var spec = new AppointmentSpecifications(A => A.Status != AppointmentStatus.Pending ||
            A.Status != AppointmentStatus.NoShow);
            var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
            return Ok(mapper.Map<List<AppointmentReadDto>>(appointments));
        }

        [HttpGet("DoctorAppoinments")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetDoctorAppointments()
        {
            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doctorId = unit.Repository<Doctor>().FindBy(D => D.UserId == doctorUserId).Select(D => D.Id).FirstOrDefault();
            var spec = new AppointmentSpecifications(A => (A.Status != AppointmentStatus.Pending ||
            A.Status == AppointmentStatus.NoShow) && A.DoctorId == doctorId);
            var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(spec);
            return Ok(mapper.Map<List<AppointmentReadDto>>(appointments));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentReadDto>> GetAppointmentById(int id)
        {
            var specs = new AppointmentSpecifications(id);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(specs);
            if (appointment is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<AppointmentReadDto>(appointment));

        }

        [HttpGet("GetByPatientId{patientId}")]
        public async Task<ActionResult<IEnumerable<PatientAppointmentDto>>> GetByPatientId(int patientId)
        {
            var patient = await unit.Repository<Appointment>().GetByIdAsync(patientId);
            var specs = new AppointmentSpecifications(A => A.PatientId == patientId);
            var appointments = await unit.Repository<Appointment>().GetAllWithSpecAsync(specs);
            if (appointments is null)
                return NotFound(new ApiResponse(404));
            return Ok(mapper.Map<IEnumerable<PatientAppointmentDto>>(appointments));

        }


        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentReadDto>> CreateAppointment([FromBody] AppointmentAddDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400));
            try
            {
                var appointment = await appointmentService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetAppointmentById),
                    new { id = appointment.Id },
                    mapper.Map<AppointmentReadDto>(appointment));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [HttpPost("ReceptionistCreate")]
        public async Task<ActionResult<AppointmentReadDto>> CreateAppointmentForReceptionist([FromBody] AppointmentAddForAnonymousDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400));
            try
            {
                var appointment = await appointmentService.CreateAsyncForReceptionist(dto);

                return CreatedAtAction(nameof(GetAppointmentById),
                    new { id = appointment.Id },
                    mapper.Map<AppointmentReadDto>(appointment));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
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
        //[Authorize(Roles = "Admin,Reception")]
        [HttpPost("NoShow/{id}")]
        public async Task<IActionResult> MarkedStatusAsNoShow(int id)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));
            var currentDateTime = DateTime.UtcNow;
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
        //[Authorize(Roles = "Patient,Reception")]
        [HttpPost("CancelAppointment/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var spec = new AppointmentSpecifications(id);
            var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(spec);



            if (appointment is null) return NotFound(new ApiResponse(404));
            if (appointment.Status == AppointmentStatus.Canceled)
                return BadRequest(new ApiResponse(400, "This appointment has already been canceled."));


            var requestTime = DateTime.UtcNow;
            if (appointment.AppointmentDateTime < requestTime)
                return BadRequest(new ApiResponse(400, "You cannot cancel an appointment that has already passed."));

            var timeBeforeCancel = appointment.AppointmentDateTime - requestTime;
            if (timeBeforeCancel >= TimeSpan.FromHours(24))
            {
                appointment.Status = AppointmentStatus.Canceled;
                unit.Repository<Appointment>().Update(appointment);
                var rowsAffected = await unit.CommitAsync();

                if (rowsAffected <= 0)
                    return BadRequest(new ApiResponse(400, "Failed to cancel appointment"));

                return Ok(new ApiResponse(200, "Appointment canceled successfully"));
            }
            return BadRequest(new ApiResponse(400, "Cannot cancel appointment within 24 hours unless marked as Emergency"));
        }

        //[Authorize(Roles = "Patient,Reception")]
        [HttpPut("RescheduleAppointment/{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentDto model)
        {
            var appointment = await unit.Repository<Appointment>().GetByIdAsync(id);
            if (appointment is null) return NotFound(new ApiResponse(404));


            if (appointment.AppointmentDateTime < DateTime.Now)
            {
                return BadRequest(new ApiResponse(400,
                    "Cannot reschedule an appointment that has already passed"));
            }

            var timeBeforeEdit = appointment.AppointmentDateTime - DateTime.Now;

            bool isLessThan24Hours = timeBeforeEdit < TimeSpan.FromHours(24);
            bool notEmergency = appointment.Status != AppointmentStatus.Emergency;

            if (isLessThan24Hours && notEmergency)
            {
                return BadRequest(new ApiResponse(400, "Cannot reschedule appointment within 24 hours unless marked as Emergency"));
            }

            var oldDateTime = appointment.AppointmentDateTime;
            appointment.AppointmentDateTime = model.NewAppointmentDateTime;

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
                NewDate = model.NewAppointmentDateTime,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = changedBy,
            };

            await unit.Repository<AppointmentAuditLog>().AddAsync(auditLog);
            unit.Repository<Appointment>().Update(appointment);
            var rowsAffected = await unit.CommitAsync();
            return rowsAffected > 0 ? Ok(new ApiResponse(200, "Appointment rescheduled successfully")) : BadRequest(new ApiResponse(400, "Failed to reschedule appointment"));

        }



        [HttpPost("ConfirmAppointment/{appointmentId}")]
        public async Task<IActionResult> ConfirmAppointment(int appointmentId)
        {

            try
            {
                var spec = new AppointmentSpecifications(a => a.Id == appointmentId);
                var appointment = await unit.Repository<Appointment>()
                    .GetByIdWithSpecAsync(spec);

                if (appointment == null)
                    return NotFound(new { error = "Appointment not found" });

                var doctor = await unit.Repository<Doctor>().GetByIdAsync(appointment.DoctorId);

                if (doctor == null)
                    return NotFound(new { error = "Doctor not found" });

                int totalAmount = (int)doctor.Price;

                var (link, billingId) = await paymentService.GetPaymentLinkAsync(appointmentId, totalAmount);
                if (string.IsNullOrEmpty(appointmentId.ToString()))
                    return BadRequest(new { error = "AppointmentId Is Required" });

                if (string.IsNullOrEmpty(link))
                    return StatusCode(500, new { error = "Cann't Create PaymentLink" });

                return Ok(new
                {
                    success = true,
                    payment_link = link,
                    totalAmount,
                    order_id = appointmentId,
                    billingId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}



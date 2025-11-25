using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;
using Sehaty.Core.Specifications.Appointment_Specs;
using Sehaty.Core.Specifications.DoctorAvailabilitySlotSpec;
using Sehaty.Core.Specifications.DoctorSpec;

namespace Sehaty.Application.Services
{
    public class DoctorAvailabilityService(IUnitOfWork unit, IMapper mapper) : IDoctorAvailabilityService
    {
        #region AddAvailabilityAsync
        public async Task AddAvailabilityAsync(CreateDoctorAvailabilityDto model)
        {

            if (model.EndTime <= model.StartTime)
                throw new Exception("End Time must be greater than Start Time");

            if (!model.IsRecurring && model.Date is null)
                throw new Exception("Date is required when the slot is not recurring");

            if (model.IsRecurring)
            {
                foreach (WeekDays day in Enum.GetValues(typeof(WeekDays)))
                {
                    if (day == WeekDays.None)
                        continue;

                    if (model.Days.HasFlag(day))
                    {
                        var slot = mapper.Map<DoctorAvailabilitySlot>(model);

                        slot.DayOfWeek = day;
                        slot.IsRecurring = true;

                        await unit.Repository<DoctorAvailabilitySlot>().AddAsync(slot);
                    }
                }
            }
            else
            {
                var slot = mapper.Map<DoctorAvailabilitySlot>(model);
                slot.IsRecurring = false;
                slot.DayOfWeek = WeekDays.None;
                slot.Date = model.Date!.Value;

                await unit.Repository<DoctorAvailabilitySlot>().AddAsync(slot);
            }

            await unit.CommitAsync();
        }
        #endregion


        #region GenerateSlots
        public async Task GenerateSlotsAsync(GenerateSlotsDto model)
        {
            await ValidateGenerateSlotRequest(model);

            var availability = await GetMatchingAvailability(model.DoctorId, model.Date);
            var slots = BuildSlots(model.DoctorId, model.Date, availability.StartTime, availability.EndTime);

            foreach (var slot in slots)
            {
                await unit.Repository<DoctorAppointmentSlot>().AddAsync(slot);
            }

            await unit.CommitAsync();
        }

        private async Task ValidateGenerateSlotRequest(GenerateSlotsDto model)
        {
            var spec = new DoctorSpecifications(model.DoctorId);
            var doctorExists = await unit.Repository<Doctor>().GetByIdWithSpecAsync(spec);
            if (doctorExists is null)
                throw new Exception("Doctor not found");


            if (model.Date < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new Exception("Date must be today or in the future");


            var specSlot = new DoctorAppointmentSlotspec(s => s.DoctorId == model.DoctorId && s.Date == model.Date);

            var existingSlots = await unit.Repository<DoctorAppointmentSlot>().GetAllWithSpecAsync(specSlot);

            if (existingSlots.Any())
                throw new Exception("Slots already exist for this doctor on this date");
        }
        private async Task<DoctorAvailabilitySlot> GetMatchingAvailability(int doctorId, DateOnly date)
        {
            var availabilitySpec = new DoctorAvailabilitySlotSpec(a =>
                a.DoctorId == doctorId && a.AvailableFlag == true);

            var allAvailability = await unit.Repository<DoctorAvailabilitySlot>().GetAllWithSpecAsync(availabilitySpec);

            if (!allAvailability.Any())
                throw new Exception("Doctor has no availability slots configured");

            var specificDateAvailability = allAvailability
                .FirstOrDefault(a => !a.IsRecurring && a.Date == date);

            if (specificDateAvailability != null)
                return specificDateAvailability;

            var dayFlag = ConvertToWeekDayFlag(date.DayOfWeek);
            var recurringAvailability = allAvailability
                .FirstOrDefault(a => a.IsRecurring && a.DayOfWeek.HasFlag(dayFlag));

            if (recurringAvailability == null)
                throw new Exception($"Doctor is not available on {date:yyyy-MM-dd} ({date.DayOfWeek})");

            return recurringAvailability;
        }
        private List<DoctorAppointmentSlot> BuildSlots(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var slots = new List<DoctorAppointmentSlot>();
            var currentTime = startTime;

            while (currentTime < endTime)
            {
                var slotEnd = currentTime.AddMinutes(30);
                if (slotEnd > endTime)
                    slotEnd = endTime;

                slots.Add(new DoctorAppointmentSlot
                {
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = currentTime,
                    EndTime = slotEnd,
                    IsBooked = false,
                    AppointmentId = null
                });

                currentTime = slotEnd;
            }

            return slots;
        }
        private WeekDays ConvertToWeekDayFlag(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => WeekDays.Saturday,
                DayOfWeek.Sunday => WeekDays.Sunday,
                DayOfWeek.Monday => WeekDays.Monday,
                DayOfWeek.Tuesday => WeekDays.Tuesday,
                DayOfWeek.Wednesday => WeekDays.Wednesday,
                DayOfWeek.Thursday => WeekDays.Thursday,
                DayOfWeek.Friday => WeekDays.Friday,
                _ => throw new Exception("Invalid day of week")
            };
        }
        #endregion

        #region GetDoctorAvailableDaysAsync
        public async Task<DoctorAvailableDaysDto> GetDoctorAvailableDaysAsync(int doctorId)
        {
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(new DoctorSpecifications(doctorId))
                ?? throw new Exception("Doctor not found");

            var availabilitySlots = await unit.Repository<DoctorAvailabilitySlot>()
                .GetAllWithSpecAsync(new DoctorAvailabilitySlotSpec(a => a.DoctorId == doctorId && a.AvailableFlag == true));

            if (!availabilitySlots.Any())
                throw new Exception("Doctor has no availability");

            var nextWeek = BuildNext7DaysSchedule(availabilitySlots);

            var specificDateSlots = availabilitySlots.Where(a => !a.IsRecurring);
            var specificDates = specificDateSlots.Select(s => new SpecificDateScheduleDto
            {
                Date = s.Date!.Value.ToString("yyyy-MM-dd"),
                StartTime = s.StartTime.ToString("hh:mm tt"),
                EndTime = s.EndTime.ToString("hh:mm tt")
            }).ToList();

            return new DoctorAvailableDaysDto
            {
                DoctorId = doctorId,
                DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                RecurringSchedule = nextWeek,
                SpecificDates = specificDates,
                AvailableDaysString = string.Join(", ", nextWeek.Where(d => d.Available == "Available").Select(d => d.Day))
            };
        }

        private List<AvailabilityScheduleDto> BuildNext7DaysSchedule(IEnumerable<DoctorAvailabilitySlot> availabilitySlots)
        {
            var result = new List<AvailabilityScheduleDto>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            for (int i = 0; i < 7; i++)
            {
                var currentDate = today.AddDays(i);
                var dayOfWeek = currentDate.DayOfWeek;
                var dayName = dayOfWeek.ToString();

                var specificSlot = availabilitySlots.FirstOrDefault(a =>
                    !a.IsRecurring && a.Date == currentDate);

                if (specificSlot != null)
                {
                    result.Add(new AvailabilityScheduleDto
                    {
                        Day = dayName,
                        Date = currentDate.ToString("yyyy/MM/dd"),
                        StartTime = specificSlot.StartTime.ToString("hh:mm tt"),
                        EndTime = specificSlot.EndTime.ToString("hh:mm tt"),
                        Available = "Available"
                    });
                    continue;
                }

                var dayFlag = ConvertToWeekDayFlags(dayOfWeek);
                var recurringSlot = availabilitySlots.FirstOrDefault(a =>
                    a.IsRecurring && a.DayOfWeek.HasFlag(dayFlag));

                if (recurringSlot != null)
                {
                    result.Add(new AvailabilityScheduleDto
                    {
                        Day = dayName,
                        Date = currentDate.ToString("yyyy/MM/dd"),
                        StartTime = recurringSlot.StartTime.ToString("hh:mm tt"),
                        EndTime = recurringSlot.EndTime.ToString("hh:mm tt"),
                        Available = "Available"
                    });
                }
                else
                {
                    result.Add(new AvailabilityScheduleDto
                    {
                        Day = dayName,
                        Date = currentDate.ToString("yyyy/MM/dd"),
                        StartTime = null,
                        EndTime = null,
                        Available = "Not Available"
                    });
                }
            }

            return result;
        }

        private WeekDays ConvertToWeekDayFlags(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => WeekDays.Saturday,
                DayOfWeek.Sunday => WeekDays.Sunday,
                DayOfWeek.Monday => WeekDays.Monday,
                DayOfWeek.Tuesday => WeekDays.Tuesday,
                DayOfWeek.Wednesday => WeekDays.Wednesday,
                DayOfWeek.Thursday => WeekDays.Thursday,
                DayOfWeek.Friday => WeekDays.Friday,
                _ => throw new Exception("Invalid day of week")
            };
        }


        #endregion

        #region GetAvailableSlotsAsync
        public async Task<GetAvailableSlotsResponseDto> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto model)
        {
            var doctorSpec = new DoctorSpecifications(model.DoctorId);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(doctorSpec);

            if (doctor is null)
                throw new Exception("Doctor not found");

            var slotsSpec = new DoctorAppointmentSlotspec(s => s.DoctorId == model.DoctorId && s.Date == model.Date);

            var allSlots = await unit.Repository<DoctorAppointmentSlot>().GetAllWithSpecAsync(slotsSpec);

            if (!allSlots.Any())
                throw new Exception($"No slots found for doctor on {model.Date:yyyy-MM-dd}. Please generate slots first.");

            var availableSlots = allSlots.Where(s => !s.IsBooked).ToList();
            var bookedSlots = allSlots.Where(s => s.IsBooked).ToList();

            var slotDtos = availableSlots
                .OrderBy(s => s.StartTime)
                .Select(s => new AvailableSlotDto
                {
                    SlotId = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    TimeRange = $"{s.StartTime:hh:mm tt} - {s.EndTime:hh:mm tt}",
                    IsBooked = s.IsBooked
                })
                .ToList();

            return new GetAvailableSlotsResponseDto
            {
                DoctorId = model.DoctorId,
                DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                Date = model.Date,
                DayOfWeek = model.Date.DayOfWeek.ToString(),
                TotalSlots = allSlots.Count(),
                AvailableSlots = availableSlots.Count,
                BookedSlots = bookedSlots.Count,
                Slots = slotDtos
            };
        }
        #endregion


        #region GetAvailableSlotsRangeAsync
        public async Task<List<GetAvailableSlotsResponseDto>> GetAvailableSlotsRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate)
        {
            var daysDifference = endDate.DayNumber - startDate.DayNumber;

            if (daysDifference < 0)
                throw new Exception("EndDate must be after or equal to StartDate");

            if (daysDifference > 7)
                throw new Exception("Cannot request slots beyond 7 days");

            var doctorSpec = new DoctorSpecifications(doctorId);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(doctorSpec);

            if (doctor is null)
                throw new Exception("Doctor not found");


            var slotsSpec = new DoctorAppointmentSlotspec(s =>
                s.DoctorId == doctorId &&
                s.Date >= startDate &&
                s.Date <= endDate);

            var allSlots = await unit.Repository<DoctorAppointmentSlot>()
                .GetAllWithSpecAsync(slotsSpec);

            var slotsByDate = allSlots.GroupBy(s => s.Date);

            var result = new List<GetAvailableSlotsResponseDto>();

            foreach (var dateGroup in slotsByDate.OrderBy(g => g.Key))
            {
                var dateSlots = dateGroup.ToList();
                var availableSlots = dateSlots.Where(s => !s.IsBooked).ToList();
                var bookedSlots = dateSlots.Where(s => s.IsBooked).ToList();

                var slotDtos = availableSlots
                    .OrderBy(s => s.StartTime)
                    .Select(s => new AvailableSlotDto
                    {
                        SlotId = s.Id,
                        Date = s.Date,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        TimeRange = $"{s.StartTime:hh:mm tt} - {s.EndTime:hh:mm tt}",
                        IsBooked = s.IsBooked
                    })
                    .ToList();

                result.Add(new GetAvailableSlotsResponseDto
                {
                    DoctorId = doctorId,
                    DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                    Date = dateGroup.Key,
                    DayOfWeek = dateGroup.Key.DayOfWeek.ToString(),
                    TotalSlots = dateSlots.Count,
                    AvailableSlots = availableSlots.Count,
                    BookedSlots = bookedSlots.Count,
                    Slots = slotDtos
                });
            }

            return result;
        }
        #endregion


        #region BookSlotAsync
        public async Task<BookSlotResponseDto> BookSlotAsync(BookSlotDto model)
        {
            var slot = await unit.Repository<DoctorAppointmentSlot>().GetByIdAsync(model.SlotId);

            if (slot is null)
                throw new Exception("Slot not found");

            if (slot.IsBooked)
                throw new Exception("This slot is already booked");

            if (slot.Date < DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Cannot book appointments in the past");

            var doctorSpec = new DoctorSpecifications(slot.DoctorId);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(doctorSpec);

            if (doctor is null)
                throw new Exception("Doctor not found");


            var patient = await unit.Repository<Patient>().GetByIdAsync(model.PatientId);

            if (patient is null)
                throw new Exception("Patient not found");

            var patientAppointmentSpec = new AppointmentSpecifications(a =>
                a.PatientId == model.PatientId &&
                a.ScheduledDate == slot.Date &&
                a.ScheduledTime == slot.StartTime &&
                a.Status != AppointmentStatus.Canceled);

            var existingAppointment = await unit.Repository<Appointment>()
                .GetByIdWithSpecAsync(patientAppointmentSpec);

            if (existingAppointment != null)
                throw new Exception("Patient already has an appointment at this time");

            var appointment = new Appointment
            {
                DoctorId = slot.DoctorId,
                PatientId = model.PatientId,
                AppointmentDateTime = slot.Date.ToDateTime(slot.StartTime),
                ScheduledDate = slot.Date,
                ScheduledTime = slot.StartTime,
                DurationMinutes = 30,
                ReasonForVisit = model.ReasonForVisit,
                Status = AppointmentStatus.Pending,
                BookingDateTime = DateTime.Now
            };

            await unit.Repository<Appointment>().AddAsync(appointment);
            await unit.CommitAsync();

            slot.IsBooked = true;
            slot.AppointmentId = appointment.Id;

            unit.Repository<DoctorAppointmentSlot>().Update(slot);
            await unit.CommitAsync();

            return new BookSlotResponseDto
            {
                AppointmentId = appointment.Id,
                SlotId = slot.Id,
                DoctorId = doctor.Id,
                DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                PatientId = patient.Id,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                AppointmentDateTime = appointment.AppointmentDateTime,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = appointment.Status.ToString()
            };
        }

        #endregion

        #region CancelSlotAsync
        public async Task CancelSlotAsync(int slotId)
        {
            var slot = await unit.Repository<DoctorAppointmentSlot>().GetByIdAsync(slotId);

            if (slot is null)
                throw new Exception("Slot not found");

            if (!slot.IsBooked)
                throw new Exception("Slot is not booked");
            var appointmentDateTime = slot.Date.ToDateTime(slot.StartTime);
            var timeUntilAppointment = appointmentDateTime - DateTime.Now;

            if (timeUntilAppointment.TotalHours < 1)
                throw new Exception("Cannot cancel appointment less than 1 hour before scheduled time");

            if (slot.AppointmentId.HasValue)
            {
                var appointment = await unit.Repository<Appointment>().GetByIdAsync(slot.AppointmentId.Value);

                if (appointment != null)
                {
                    if (appointment.Status == AppointmentStatus.Completed)
                        throw new Exception("Cannot Cancel slot for completed appointment");

                    if (appointment.Status == AppointmentStatus.InProgress)
                        throw new Exception("Cannot Cancel slot for in-progress appointment");

                    appointment.Status = AppointmentStatus.Canceled;
                    unit.Repository<Appointment>().Update(appointment);
                }
            }
            slot.IsBooked = false;
            slot.AppointmentId = null;
            unit.Repository<DoctorAppointmentSlot>().Update(slot);
            await unit.CommitAsync();
        }

        #endregion

        #region GetSlotDetailsAsync
        public async Task<SlotDetailsDto> GetSlotDetailsAsync(int slotId)
        {
            var slot = await unit.Repository<DoctorAppointmentSlot>().GetByIdAsync(slotId);

            if (slot is null)
                throw new Exception("Slot not found");

            var doctorSpec = new DoctorSpecifications(slot.DoctorId);
            var doctor = await unit.Repository<Doctor>().GetByIdWithSpecAsync(doctorSpec);

            AppointmentDetailsDto appointmentDetails = null;

            if (slot.IsBooked && slot.AppointmentId.HasValue)
            {
                var appointmentSpec = new AppointmentSpecifications(slot.AppointmentId.Value);
                var appointment = await unit.Repository<Appointment>().GetByIdWithSpecAsync(appointmentSpec);

                if (appointment != null)
                {
                    appointmentDetails = new AppointmentDetailsDto
                    {
                        AppointmentId = appointment.Id,
                        PatientId = appointment.PatientId,
                        PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                        ReasonForVisit = appointment.ReasonForVisit,
                        Status = appointment.Status.ToString(),
                        BookingDateTime = appointment.BookingDateTime
                    };
                }
            }

            return new SlotDetailsDto
            {
                SlotId = slot.Id,
                DoctorId = slot.DoctorId,
                DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                Date = slot.Date,
                DayOfWeek = slot.Date.DayOfWeek.ToString(),
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                TimeRange = $"{slot.StartTime:hh:mm tt} - {slot.EndTime:hh:mm tt}",
                IsBooked = slot.IsBooked,
                Appointment = appointmentDetails
            };
        }
        #endregion
    }
}

namespace Sehaty.APIs.Controllers
{
    public class DoctorAvailabilitySlotsController(IDoctorAvailabilityService availabilityService) : ApiBaseController //IUnitOfWork unit, IMapper mapper,
    {


        [HttpPost("AddAvailabilitySlot")]
        public async Task<IActionResult> AddAvailabilitySlot([FromBody] CreateDoctorAvailabilityDto model)
        {
            try
            {
                await availabilityService.AddAvailabilityAsync(model);
                return Ok(new ApiResponse(200, "Doctor availability slot(s) added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, $"{ex.Message}"));
            }
        }

        [HttpPost("Generateslots")]
        public async Task<IActionResult> GenerateSlots([FromBody] GenerateSlotsDto model)
        {
            try
            {
                await availabilityService.GenerateSlotsAsync(model);
                return Ok(new ApiResponse(200, "Slots generated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, $"{ex.Message}"));
            }
        }



        [HttpGet("slotDetails/{slotId}")]
        public async Task<IActionResult> GetSlotDetails(int slotId)
        {
            try
            {
                var result = await availabilityService.GetSlotDetailsAsync(slotId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }



        [HttpGet("availableDaysForDoctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorAvailableDays(int doctorId)
        {
            try
            {
                var result = await availabilityService.GetDoctorAvailableDaysAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }



        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateOnly date)
        {
            try
            {
                var model = new GetAvailableSlotsRequestDto
                {
                    DoctorId = doctorId,
                    Date = date
                };

                var result = await availabilityService.GetAvailableSlotsAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }


        [HttpPost("BookSlot")]
        public async Task<IActionResult> BookSlot([FromBody] BookSlotDto model)
        {
            try
            {
                var result = await availabilityService.BookSlotAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }


        [HttpGet("AvailableSlotsRangeForDoctor")]
        public async Task<IActionResult> GetAvailableSlotsRange([FromQuery] int doctorId, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            try
            {
                var result = await availabilityService.GetAvailableSlotsRangeAsync(
                    doctorId,
                    startDate,
                    endDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [HttpPost("Cancelslot/{slotId}")]
        public async Task<IActionResult> CancelSlot(int slotId)
        {
            try
            {
                await availabilityService.CancelSlotAsync(slotId);
                return Ok(new ApiResponse(200, "Slot Canceled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

    }
}

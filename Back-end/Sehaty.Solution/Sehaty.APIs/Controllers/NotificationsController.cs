namespace Sehaty.APIs.Controllers
{

    public class NotificationsController(IUnitOfWork unit, IMapper map) : ApiBaseController
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = new NotificationSpecifications();
            var notifications = await unit.Repository<Notification>().GetAllWithSpecAsync(spec);
            if (notifications != null)
                return Ok(map.Map<IEnumerable<AllNotificationsDto>>(notifications));
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = new NotificationSpecifications(id);
            var notification = await unit.Repository<Notification>().GetByIdWithSpecAsync(spec);
            if (notification != null)
                return Ok(map.Map<AllNotificationsDto>(notification));
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("GetByPatientId")]
        public async Task<ActionResult<IEnumerable<AllNotificationsDto>>> GetByPatientId()
        {
            var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var patientId = (await unit.Repository<Patient>().GetFirstOrDefaultAsync(P => P.UserId == patientUserId)).Id;
            var spec = new NotificationSpecifications(N => N.UserId == patientUserId);
            var notifications = await unit.Repository<Notification>().GetAllWithSpecAsync(spec);
            if (notifications.Count() > 0)
                return Ok(map.Map<IEnumerable<AllNotificationsDto>>(notifications));
            return NotFound(new ApiResponse(404));
        }
        [HttpGet("Unread")]
        public async Task<ActionResult<IEnumerable<AllNotificationsDto>>> GetUnreadedByPatientId()
        {
            var patientUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var patientId = (await unit.Repository<Patient>().GetFirstOrDefaultAsync(P => P.UserId == patientUserId)).Id;
            var spec = new NotificationSpecifications(N => N.UserId == patientUserId && !N.IsRead);
            var notifications = await unit.Repository<Notification>().GetAllWithSpecAsync(spec);
            //var unreadnotifications = notifications.Where(n => !n.IsRead);
            if (notifications.Count() > 0)
                return Ok(map.Map<IEnumerable<AllNotificationsDto>>(notifications));
            return NotFound(new ApiResponse(404));
        }
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createNotificationDto)
        {
            if (ModelState.IsValid)
            {
                var notification = map.Map<Notification>(createNotificationDto);
                await unit.Repository<Notification>().AddAsync(notification);
                await unit.CommitAsync();
                return CreatedAtAction(nameof(GetById), new { id = notification.Id }, map.Map<AllNotificationsDto>(notification));
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] CreateNotificationDto updateNotificationDto)
        {
            if (ModelState.IsValid)
            {
                var spec = new NotificationSpecifications(id);
                var notification = await unit.Repository<Notification>().GetByIdWithSpecAsync(spec);
                if (notification == null)
                    return NotFound(new ApiResponse(404));

                map.Map(updateNotificationDto, notification);
                unit.Repository<Notification>().Update(notification);
                await unit.CommitAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var spec = new NotificationSpecifications(id);
            var notification = await unit.Repository<Notification>().GetByIdWithSpecAsync(spec);
            if (notification == null)
                return NotFound(new ApiResponse(404));

            unit.Repository<Notification>().Delete(notification);
            await unit.CommitAsync();
            return NoContent();
        }




    }
}

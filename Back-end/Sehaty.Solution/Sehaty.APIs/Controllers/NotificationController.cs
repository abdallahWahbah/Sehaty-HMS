using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.NotificationsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Specifications.Notifications_Specs;
using Sehaty.Core.UnitOfWork.Contract;

namespace Sehaty.APIs.Controllers
{

    public class NotificationController : ApiBaseController
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper map;

        public NotificationController(IUnitOfWork unit, IMapper map)
        {
            this.unit = unit;
            this.map = map;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = new NotificationSpecifications();
            var notifications = await unit.Repository<Notification>().GetAllWithSpecAsync(spec);
            if (notifications != null)
                return Ok(map.Map<IEnumerable<AllNotificationsDto>>(notifications));
            return NotFound();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = new NotificationSpecifications(id);
            var notification = await unit.Repository<Notification>().GetByIdWithSpecAsync(spec);
            if (notification != null)
                return Ok(map.Map<AllNotificationsDto>(notification));
            return NotFound();
        }
        [HttpGet("GetByPatientId/{id}")]
        public async Task<ActionResult<IEnumerable<AllNotificationsDto>>> GetByPatientId(int id)
        {
            var spec = new NotificationSpecifications(N => N.UserId == id);
            var notifications = await unit.Repository<Notification>().GetAllWithSpecAsync(spec);
            if (notifications != null)
                return Ok(map.Map<IEnumerable<AllNotificationsDto>>(notifications));
            return NotFound();
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
                    return NotFound();

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
                return NotFound();

            unit.Repository<Notification>().Delete(notification);
            await unit.CommitAsync();
            return NoContent();
        }
    }
}

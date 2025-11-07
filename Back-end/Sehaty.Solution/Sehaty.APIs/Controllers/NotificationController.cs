using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sehaty.Application.Dtos.NotificationsDTOs;
using Sehaty.Application.Dtos.PrescriptionsDTOs;
using Sehaty.Core.Entites;
using Sehaty.Core.Entities.Business_Entities;
using Sehaty.Core.Specifications.Notifications_Specs;
using Sehaty.Core.UnitOfWork.Contract;
using System.Collections.Generic;

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
                return Ok(map.Map< IEnumerable < AllNotificationsDto >> (notifications));
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            var notification = map.Map<Notification>(createNotificationDto);
            await unit.Repository<Notification>().AddAsync(notification);
            await unit.CommitAsync();
            return Ok();
        }
        }
}

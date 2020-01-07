using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Services.Interface;
using Chefbook.API.SignalR.Abstract;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    //    [Authorize]
    public class NotificationController : ControllerBase
    {
        private IHubNotification _hubNotification;
        private INotificationService _notificationService;

        public NotificationController(IHubNotification hubNotification, INotificationService notificationService)
        {
            _hubNotification = hubNotification;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Route("test")]
        public async Task<ActionResult> Get(Noti model)
        {
            await this._hubNotification.Send(model.gondericiId, model.aliciId, model.message); //gelen verileri hub gönderip veritabanına kaydet sonra Clienta gönder.
            this._notificationService.Add(new Notification{Id = new Guid(),UserId = model.gondericiId,TriggerUserId = model.aliciId,NotificationDescription = model.message});


            return StatusCode(200,"Test başarılı");

        }
        [HttpGet]
        [Route("getnotification")]
        public IActionResult GetNotification()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var notification = _notificationService.Get(i => i.UserId == Guid.Parse(currentUserId));
            if (notification!=null)
            {
                return StatusCode(200, notification);

            }
            else
            {
                return StatusCode(301, "Notification Yok");
            }
          
        }
        [HttpGet]
        [Route("readnotification/{notificationId}")]
        public IActionResult ReadNotification(Guid notificationId)
        {
            var notification = _notificationService.GetById(notificationId);
            return StatusCode(200, notification);


        }

    public class Noti
    {
        public Guid aliciId { get; set; }
        public Guid gondericiId { get; set; }
        public string message { get; set; }
    }
}
}
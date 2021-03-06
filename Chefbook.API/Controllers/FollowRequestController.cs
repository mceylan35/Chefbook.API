﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.SignalR.Concrete;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowRequestController : ControllerBase
    {
        
        private IUserService _userService;
        private IFollowService _followService;
        private INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public FollowRequestController(IHubContext<NotificationHub> hubContext,IUserService userService, IFollowService followService, INotificationService notificationService)
        {
             _hubContext = hubContext;
            _userService = userService;
            _followService = followService;
            _notificationService = notificationService;
        }
    
        [Route("follow")]
        [HttpPost]
        public ActionResult FollowUser(UserFollowDTO model)
        {
          
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (!_userService.Exists(model.FollowingId) && !_userService.Exists(model.FollowersId))
                {
                    return NotFound("Kullanıcı yok");
                }
                //önceden takip ediyormuyum?
               
                if (_followService.AmIFollowing(model.FollowersId,model.FollowingId))
                {
                    return BadRequest("Zaten Takip ediyorsun");
                }
                _followService.Add(new Follow { Id = Guid.NewGuid(), FollowDate = DateTime.Now, FollowersId = model.FollowersId, FollowingId = model.FollowingId });
                _notificationService.Create(new Notification
                {
                    Id = Guid.NewGuid(),UserId = Guid.Parse(currentUserId),TriggerUserId = model.FollowersId
                });
                _hubContext.Clients.All.SendAsync("NotificationGuncelle");
               
             //  _hubContext.Clients.User("P0m3D-Y_Ra5VZBd3ypUR5g").SendAsync("SendNotification", Guid.Parse("4bcbbcbf-d75f-4c0f-821c-2a833f800ff4"), Guid.Parse("6f6dc2ff-b828-41d1-a27b-a993e944ce7e"), "asdsad");

               return StatusCode(200, "Başarılı");


            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
           
        }
        [Route("removefollow")]
        [HttpPost]
        public IActionResult RemoveFollow(RemoveFollowDTO model)
        {
            try
            {
                if (!_userService.Exists(model.FollowingId) && !_userService.Exists(model.FollowersId))
                {
                    return NotFound("Kullanıcı yok");
                }
                //önceden takip ediyormuyum?

                if (_followService.AmIFollowing(model.FollowersId, model.FollowingId))
                {
                    var removefollow = _followService.Get(i =>
                        i.FollowingId == model.FollowersId && i.FollowersId == model.FollowersId);
                    _followService.Delete(removefollow);
                    return Ok("Başarılı");
                }

                return BadRequest("Bir sıkıntı oluştu!");


            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private IFollowService _followService;
        private INotificationService _notificationService;

        public FollowController(IFollowService followService, INotificationService notificationService)
        {
            _followService = followService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        [Route("getfollowers")]
        public ActionResult GetFollowers()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var getFollowers = _followService.GetFollowersUser(Guid.Parse(currentUserId), 0, 25);
                return Ok(getFollowers);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}
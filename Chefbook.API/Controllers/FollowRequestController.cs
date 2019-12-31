using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public FollowRequestController(IUserService userService, IFollowService followService)
        {
            _userService = userService;
            _followService = followService;
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

                return Ok("Success");
                // return Ok("Başarılı");

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private IPostService _postService;
        private ICommentService _commentService;

        public CommentController(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        [HttpPost]

        [Route("add")]
        [Authorize]
        public IActionResult Create(PostCommentAddDTO model)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Yorum Boş Olamaz!");
                    return BadRequest(ModelState);
                }

                if (_postService.Exists(model.PostId))
                {
                    _commentService.Add(new Comment {Id = Guid.NewGuid(),CommentDate = DateTime.Now, Description = model.Description, PostId = model.PostId,UserId = Guid.Parse(currentUserId) });
                    return Ok("Yorum Eklendi");
                }

                return NotFound(new {data = "Böyle bir post yok"});
            }
            catch (Exception e)
            {
               
                return BadRequest(e);
            }


        }
        [HttpDelete]
        [Route("sil/{commentId}")]
        public IActionResult DeleteCommentByPostId(Guid commentId)
        {
            try
            {
                //kendi yorumu ise silsin onu düzenle
                var comment= _commentService.GetById(commentId);
                if (comment != null)
                {
                    _commentService.Delete(comment);
                    return Ok("Silme İşlemi Başarılı");
                }

                return BadRequest("Böyle bir yorum yok");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


    }
}
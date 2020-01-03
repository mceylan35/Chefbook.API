using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Helpers;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.Model.DTO;
using Chefbook.Model.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private IPostService _postService;
        private IStepService _stepService;
        private IImageService _imageService;
        private ILikeService _likeService;
        private IOptions<CloudinarySettings> _cloudinaryConfig;
        private IStarService _starService;


        private Cloudinary _cloudinary;

        public PostController(IStarService starService, IPostService postService, IStepService stepService,
            IImageService imageService, ILikeService likeService, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _postService = postService;
            _stepService = stepService;
            _imageService = imageService;
            _likeService = likeService;
            _starService = starService;
            _cloudinaryConfig = cloudinaryConfig;
            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);


        }




        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromForm] PostDTO model)
        {
            try
            {
                // _postService.BeginTransaction();
                // _imageService.BeginTransaction();
                // _stepService.BeginTransaction();
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                Guid guid = Guid.NewGuid();



                if (model.Photos.Any(f => f.Length == 0))
                {
                    return BadRequest();
                }

                var images = new List<Image>();

                foreach (var file in model.Photos)
                {


                    var uploadResult = new ImageUploadResult();
                    if (file.Length > 0)
                    {
                        using (var stream = file.OpenReadStream())
                        {

                            var uploadParams = new ImageUploadParams
                            {
                                File = new FileDescription(file.Name, stream),


                            };
                            uploadResult = _cloudinary.Upload(uploadParams);

                        }
                    }



                    images.Add(new Image
                    {
                        Id = Guid.NewGuid(), ImageUrls = uploadResult.Uri.ToString(), PostId = guid,
                        PublicId = uploadResult.PublicId
                    });



                }



                _imageService.AddRange(images);
                if (model.Steps != null)
                {
                    List<Step> steps = new List<Step>();
                    foreach (var step in model.Steps)
                    {
                        steps.Add(new Step {Id = Guid.NewGuid(), PostId = guid, Description = step.Description});

                    }

                    _stepService.AddRange(steps);
                }




                _postService.Add(new Post
                {
                    Id = guid, Description = model.Description, UserId = Guid.Parse(currentUserId),
                    PostDate = DateTime.Now, LikeCount = 0, Title = model.Title,RateSum = 0
                });


                //_postService.CommitTransaction();
                // _imageService.CommitTransaction();
                // _stepService.CommitTransaction();

                return Ok("Success");
            }
            catch (Exception e)
            {
                // _postService.RollbackTransaction();
                //_imageService.RollbackTransaction();
                // _stepService.RollbackTransaction();
                return BadRequest(e);
            }

        }

        [HttpGet]
        [Route("delete/{postId}")]
        public IActionResult DeletePost(Guid postId)
        {
            try
            {
                var deletingpost = _postService.GetById(postId);
                if (deletingpost != null)
                {
                    _postService.Delete(deletingpost);
                    return Ok("success");
                }


                return BadRequest("Post Silinmedi.");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("like/{postId}")]
        public IActionResult LikePost(Guid postId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (_postService.Like(postId) && !_likeService.BegendinMi(Guid.Parse(currentUserId), postId))
                {
                    _likeService.Add(new Like
                    {
                        Id = Guid.NewGuid(),
                        PostId = postId,
                        BegenenId = Guid.Parse(currentUserId)
                    });
                    return Ok("Başarılı");
                }
                else
                {
                    return NotFound("Zaten beğenmişsiniz.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }


        }

        [HttpGet]
        [Route("dislike/{postId}")]
        public IActionResult DisLikePost(Guid postId)
        {

            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (_postService.DisLike(postId))
                {

                    //dislike sorunlu

                    var dislike = _likeService.Get(i => i.BegenenId == Guid.Parse(currentUserId) && i.PostId == postId);

                    _likeService.Delete(dislike);
                    return Ok("Dislike Başarılı");
                }
                else
                {
                    return NotFound("Dislike başarısız.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }



        }

        [HttpPost]
        [Route("starAdd")]
        public IActionResult StarAdd(StarDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var star = _starService.Get(i => i.UserId == Guid.Parse(currentUserId) && i.PostId == model.PostId);

            if (star==null)
            {
                _starService.Add(new Star
                {
                    Id = new Guid(),
                    PostId = model.PostId,
                    RateNumber = model.RateNumber,
                    UserId = Guid.Parse(currentUserId)
                });
                return StatusCode(200, "Star Başarılı");
            }
            else
            {
                star.RateNumber = model.RateNumber;
                _starService.Update(star);

                return StatusCode(201, "Star Güncellendi."); ;
            }
           
            
        }
    }
}
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
        private IIngredientService _ingredientService;
        private ICommentService _commentService;

        private Cloudinary _cloudinary;

        public PostController(IStarService starService, IPostService postService, IStepService stepService,
            IIngredientService ingredientService,
            IImageService imageService, ILikeService likeService, IOptions<CloudinarySettings> cloudinaryConfig,
            ICommentService commentService)
        {
            _postService = postService;
            _stepService = stepService;
            _imageService = imageService;
            _likeService = likeService;
            _starService = starService;
            _commentService = commentService;
            _ingredientService = ingredientService;
            _cloudinaryConfig = cloudinaryConfig;
            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok(_imageService.GetAll().Select(i=>i.ImageUrls));
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromForm] PostDTO model)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var guid = Guid.NewGuid();

                if (model.Photos.Count == 0)
                {
                    return StatusCode(397);
                }

                if (model.Photos == null)
                {
                    return StatusCode(398);
                }

                var images = new List<Image>();

                foreach (var file in model.Photos)
                {
                    var extention = Path.GetExtension(file.FileName);
                    if (extention == ".jpg" || extention == ".png")
                    {
                        var uploadResult = new ImageUploadResult();

                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams
                            {
                                File = new FileDescription(file.Name, stream),
                            };
                            uploadResult = _cloudinary.Upload(uploadParams);
                        }

                        images.Add(new Image
                        {
                            Id = Guid.NewGuid(),
                            ImageUrls = uploadResult.Uri.ToString(),
                            PostId = guid,
                            PublicId = uploadResult.PublicId
                        });
                    }

                    if (extention == ".mp4")
                    {
                        var uploadvideoResult = new VideoUploadResult();
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new VideoUploadParams()
                            {
                                File = new FileDescription(file.Name, stream)
                            };
                            uploadvideoResult = _cloudinary.Upload(uploadParams);
                        }

                        images.Add(new Image
                        {
                            Id = Guid.NewGuid(),
                            ImageUrls = uploadvideoResult.Uri.ToString(),
                            PostId = guid,
                            PublicId = uploadvideoResult.PublicId
                        });
                    }
                }


                _imageService.AddRange(images);
                if (model.Steps != null)
                {
                    List<Step> steps = new List<Step>();
                    foreach (var step in model.Steps)
                    {
                        steps.Add(new Step {Id = Guid.NewGuid(), PostId = guid, Description = step});
                    }

                    _stepService.AddRange(steps);
                }

                if (model.Ingredients != null)
                {
                    List<Ingredients> ingredientses = new List<Ingredients>();
                    foreach (var ingredient in model.Ingredients)
                    {
                        ingredientses.Add(new Ingredients
                            {Id = Guid.NewGuid(), PostId = guid, Ingredient = ingredient});
                    }

                    _ingredientService.AddRange(ingredientses);
                }


                _postService.Add(new Post
                {
                    Id = guid, Description = model.Description, UserId = Guid.Parse(currentUserId),
                    PostDate = DateTime.Now, LikeCount = 0, Title = model.Title, StarGivenUserCount = 0, SumStar = 0,
                    Star = 0
                });


                return Ok("Success");
            }
            catch (Exception e)
            {
                return StatusCode(399);
            }
        }

        [HttpPost]
        [Route("addpost")]
        public IActionResult AddPost(string title, string description, string[] steps, string[] ingredients, [FromForm] IFormFile[] photos)
        {
            try
            {
                if (steps == null || steps.Length == 0)
                {
                    return StatusCode(395);
                }

                if (ingredients == null || ingredients.Length == 0)
                {
                    return StatusCode(396);
                }

                if (photos == null || photos.Length == 0)
                {
                    return StatusCode(397);
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var guid = Guid.NewGuid();

                var images = new List<Image>();

                foreach (var file in photos)
                {
                    var extention = Path.GetExtension(file.FileName);
                    if (extention != ".jpg" && extention != ".png") continue;

                    ImageUploadResult uploadResult;

                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.Name, stream),
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }

                    images.Add(new Image
                    {
                        Id = Guid.NewGuid(),
                        ImageUrls = uploadResult.Uri.ToString(),
                        PostId = guid,
                        PublicId = uploadResult.PublicId
                    });
                }
                _imageService.AddRange(images);

                var stepler = steps.Select(step => new Step {Id = Guid.NewGuid(), PostId = guid, Description = step}).ToList();
                _stepService.AddRange(stepler);


                var ingredientses = ingredients.Select(ingredient => new Ingredients {Id = Guid.NewGuid(), PostId = guid, Ingredient = ingredient}).ToList();
                _ingredientService.AddRange(ingredientses);

                _postService.Add(new Post
                {
                    Id = guid,
                    Description = description,
                    UserId = Guid.Parse(currentUserId),
                    PostDate = DateTime.Now,
                    LikeCount = 0,
                    Title = title,
                    StarGivenUserCount = 0,
                    SumStar = 0,
                    Star = 0
                });

                return Ok("Success");
            }
            catch (Exception e)
            {
                return StatusCode(399);
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
                return StatusCode(399, "hata");
            }
        }

        [HttpGet]
        [Route("details/{postId}")]
        public IActionResult Detay(Guid postId)
        {
            var detay = _postService.Detay(postId);
            return Ok(detay);
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

            var post = _postService.GetById(model.PostId);
            if (star == null)
            {
                post.StarGivenUserCount += 1;
                post.SumStar += model.RateNumber;
                post.Star = post.SumStar / post.StarGivenUserCount;
                _postService.Update(post);
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
                post.SumStar += model.RateNumber - star.RateNumber;
                post.Star = post.SumStar / post.StarGivenUserCount;
                _postService.Update(post);
                star.RateNumber = model.RateNumber;
                _starService.Update(star);

                return StatusCode(201, "Star Güncellendi.");
                ;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Helpers;
using Chefbook.API.Models;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoryController : ControllerBase
    {
        private IStoryService _storyService;

        private IOptions<CloudinarySettings> _cloudinaryConfig;

        private Cloudinary _cloudinary;

        public StoryController(IStoryService storyService, IOptions<CloudinarySettings> cloudinaryConfig
            )
        {
            _storyService = storyService;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult StoryAdd([FromForm] List<IFormFile> model)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (model.Any(f => f.Length == 0))
                {
                    return StatusCode(301, "Resim veya Video Yok");
                }

                var stories = new List<Story>();

                foreach (var file in model)
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


                        stories.Add(new Story()
                        {
                            Id = Guid.NewGuid(),
                            StoryUrl = uploadResult.Uri.ToString(),
                            UserId = Guid.Parse(currentUserId),
                            PublicId = uploadResult.PublicId,
                            CreatedDate = DateTime.Now,
                            IsActive = true
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

                        stories.Add(new Story()
                        {
                            Id = Guid.NewGuid(),
                            StoryUrl = uploadvideoResult.Uri.ToString(),
                            UserId = Guid.Parse(currentUserId),
                            PublicId = uploadvideoResult.PublicId,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        });
                    }

                }

                _storyService.AddRange(stories);

                return StatusCode(200, "Başarılı");
            }
            catch (Exception e)
            {
                return StatusCode(302, "Hata oluştu");
            }
        }

        [HttpGet]
        [Route("mystories")]
        public IActionResult MyStories()
        {
            
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var stories = _storyService.Stories(Guid.Parse(currentUserId));


                return StatusCode(200,stories.ToList());

          
        }




    }




}
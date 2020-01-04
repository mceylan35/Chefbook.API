using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Helpers;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.Services.RepositoryServices;
using Chefbook.Model;
using Chefbook.Model.DTO;
using Chefbook.Model.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Chefbook.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        private IFollowService _followService;
        private IPostService _postService;
        private IImageService _imageService;
        private IStarService _starService;
        private IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public UserController(IUserService userService, IConfiguration configuration, IStarService starService, IFollowService followService, IPostService postService, IImageService imageService, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userService = userService;
            _configuration = configuration;
            _followService = followService;
            _postService = postService;
            _imageService = imageService;
            _cloudinaryConfig = cloudinaryConfig;
            _starService = starService;

            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _userService.UserExists(user.UserName))
            {
                ModelState.AddModelError("Mail", "Mail already exists");
            }
            try
            {
                var userToCreate = new User()
                {
                    Mail = user.Mail,
                    NameSurName = user.NameSurName,
                    RegisterDate = DateTime.Now,
                    UserName = user.UserName,
                    //  Birthday = user.Birthday,
                    // Gender = user.Gender,
                    Id = Guid.NewGuid(),
                };
                var createdUser = await _userService.Register(userToCreate, user.Password);
                var tokenString = GetToken(userToCreate.Id, userToCreate.Mail);
                return Ok(tokenString);
            }
            catch (Exception)
            {

                throw;
            }


        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _userService.Login(login.Email, login.Password);
            if (user == null)
            {
                return Unauthorized();

            }

            string tokenString = GetToken(user.Id, login.Email);
            return Ok(tokenString); //þifrelenmiþ tokený gönderiyoruz. 


        }

        string GetToken(Guid userId, string mail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userId.ToString()),//kullanýcý Ýd tutuyoruz
                    new Claim(ClaimTypes.Name,mail) // kullanýcý maili tutuyoruz
              
                }),
                Expires = DateTime.Now.AddDays(30),//1gün oturum süresi
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature) //þifreleme kýsmý
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); //tokený üret
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
        [HttpGet]
        [Route("liste")]
        public ActionResult Liste()
        {

            try
            {
                _userService.BeginTransaction();
                var test = _userService.GetInclude(includes: sources => sources.Include(x => x.Comment)).ToList();
                _userService.CommitTransaction();
                return Ok(test);
            }
            catch (Exception e)
            {
                _userService.RollbackTransaction();
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [Route("search/{q}")]
        public ActionResult Search(string q)
        {
            try
            {
                var kullanicilar = _userService.Users(q);
                if (kullanicilar != null)
                {
                    return Ok(kullanicilar);
                }

                return NotFound("Kullanýcý Bulunamadý!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public IActionResult Profile()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var kullanici = _userService.GetById(Guid.Parse(currentUserId));
                long followersCount = _followService.GetFollowersCount(Guid.Parse(currentUserId));
                long following = _followService.GetFollowingCount(Guid.Parse(currentUserId));


                var posts = _postService.GetAll(i => i.UserId == Guid.Parse(currentUserId));


                ProfileDto profileDto = new ProfileDto();

                profileDto.UserName = kullanici.UserName.Trim();
                profileDto.Cover = kullanici.CoverImage;
                profileDto.Description = kullanici.Description;
                profileDto.FullName = kullanici.NameSurName.Trim();
                profileDto.FollowerCount = followersCount;
                profileDto.PostCount = posts.Count;
                profileDto.ProfilePicture = kullanici.ProfileImage;
                profileDto.ProfilePosts = _imageService.FindImage(Guid.Parse(currentUserId));


                return Ok(profileDto);
                //Sayfaya göre deðiþecek.


                // return NotFound("Kullanýcý Bulunamadý!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("wall")]
        public IActionResult Wall()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var walluser = _userService.Wall(Guid.Parse(currentUserId));
            foreach (var model in walluser)
            {

                model.StarNumber = model.StarNumber / _starService.GetAll().Count(i => i.Id == model.StarId);

            }
            return Ok(walluser);
        }

        [HttpPut]
        [Route("coverupdate")]
        public IActionResult CoverUpdate([FromForm]IFormFile model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var uploadResult = new ImageUploadResult();
            if (model.Length > 0)
            {
                using (var stream = model.OpenReadStream())
                {

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.Name, stream),


                    };
                    uploadResult = _cloudinary.Upload(uploadParams);

                }

                var user = _userService.GetById(Guid.Parse(currentUserId));
                user.CoverImage = uploadResult.Uri.ToString();
                _userService.Update(user);
                return StatusCode(200, "Cover Güncellendi.");
            }
            else
            {
                return StatusCode(301, "Resim Yok");
            }










        }

        [HttpPut]
        [Route("profileimageupdate")]
        public IActionResult ProfileImageUpdate([FromForm]IFormFile model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;




            var uploadResult = new ImageUploadResult();
            if (model.Length > 0)
            {
                using (var stream = model.OpenReadStream())
                {

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.Name, stream),


                    };
                    uploadResult = _cloudinary.Upload(uploadParams);

                }

                var user = _userService.GetById(Guid.Parse(currentUserId));
                user.ProfileImage = uploadResult.Uri.ToString();
                _userService.Update(user);
                return StatusCode(200, "Resim Güncellendi.");
            }
            else
            {
                return StatusCode(301, "Resim Yok.");
            }






        }

        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var response = await _userService.ChangePassword(changePasswordViewModel, Guid.Parse(currentUserId));
                    if (response)
                    {
                        return StatusCode(200, "Þifre Deðiþtirildi.");
                    }
                    // ModelState.AddModelError("ChangePasswordError","Þifre Deðiþtirilemedi.");
                    return StatusCode(301, "Eski Þifre Yanlýþ");
                  
                }
                catch (Exception x)
                {
                    ModelState.AddModelError("ChangePasswordError", x.ToString()); // TODO: Replace x with your error message
                    return StatusCode(302, "Þifreler ayný deðil.");
                }
            }

            return StatusCode(302, "Bir hata  oluþtu.");

        }






        [HttpGet("getprofiledetails")]
        [Authorize]
        public IActionResult ChangeProfileGet()
        {

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userService.GetById(Guid.Parse(currentUserId));
            if (user != null)
            {

                return StatusCode(200, new ChangeInformationProfileViewModel
                {
                    NameSurName = user.NameSurName,
                    Mail = user.Mail,
                    UserName = user.UserName,
                    Biography = user.Description
                });


            }
            return StatusCode(301, "hata Oluþtu");
            //return BadRequest("Bir hata oluþtu.");
        }

        [HttpPost("changeprofile")]
        [Authorize]
        public IActionResult ChangeProfile(ChangeInformationProfileViewModel model)
        {


            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _userService.GetById(Guid.Parse(currentUserId));

                if (user.UserName != model.UserName || user.Mail != model.Mail || user.NameSurName != model.NameSurName || user.Description != model.Biography)
                {




                    if (user.UserName != model.UserName)
                    {
                        var isExistingUserName = _userService.UserExists(model.UserName);

                        if (isExistingUserName.Result)
                        {
                            return StatusCode(301, "Böyle bir UserName var.");
                        }
                        else
                        {
                            user.UserName = model.UserName;
                        }
                    }

                    if (user.Mail != model.Mail)
                    {
                        var isExistingMail = _userService.MailExists(model.Mail);

                        if (isExistingMail.Result)
                        {
                            return StatusCode(302, "Böyle bir Mail var.");
                        }
                        else
                        {
                            user.Mail = model.Mail;
                        }
                    }

                    if (user.Description != model.Biography)
                        user.Description = model.Biography;
                    if (user.NameSurName != model.NameSurName)
                        user.NameSurName = model.NameSurName;
                    _userService.Update(user);

                    return StatusCode(200, "Bilgiler Güncellendi.");

                }
                else
                {
                    return StatusCode(304, "Bilgiler Güncellenmedi.");
                }
               

            }
            return BadRequest("Hata oluþtu");
        }





    }
}

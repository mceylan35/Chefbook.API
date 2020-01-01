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
        private IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public UserController(IUserService userService, IConfiguration configuration, IFollowService followService, IPostService postService, IImageService imageService, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userService = userService;
            _configuration = configuration;
            _followService = followService;
            _postService = postService;
            _imageService = imageService;
            _cloudinaryConfig = cloudinaryConfig;
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
            if (await _userService.UserExists(user.Mail))
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
            return Ok(tokenString); //�ifrelenmi� token� g�nderiyoruz. 


        }

        string GetToken(Guid userId, string mail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userId.ToString()),//kullan�c� �d tutuyoruz
                    new Claim(ClaimTypes.Name,mail) // kullan�c� maili tutuyoruz
              
                }),
                Expires = DateTime.Now.AddDays(30),//1g�n oturum s�resi
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature) //�ifreleme k�sm�
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); //token� �ret
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
                if (kullanicilar!=null)
                {
                    return Ok(kullanicilar);
                }

                return NotFound("Kullan�c� Bulunamad�!");
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
                profileDto.Cover= "https://i.nefisyemektarifleri.com/2018/05/04/mercimek-corbasi-tarifi.jpg";
                profileDto.Description = "Veritaban�na Eklenecek";
                profileDto.FullName = kullanici.NameSurName.Trim();
                profileDto.FollowerCount = followersCount;
                profileDto.PostCount = posts.Count;
                profileDto.ProfilePicture= "https://i.nefisyemektarifleri.com/2018/05/04/mercimek-corbasi-tarifi.jpg";
                profileDto.ProfilePosts = _imageService.FindImage(Guid.Parse(currentUserId));

                
                    return Ok(profileDto);
                    //Sayfaya g�re de�i�ecek.
                

               // return NotFound("Kullan�c� Bulunamad�!");
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


                    }

                    var user = _userService.GetById(Guid.Parse(currentUserId));
                    user.CoverImage = uploadResult.Uri.ToString();
                    _userService.Update(user);
                    return Ok("Cover G�ncellendi.");
                }
                else
                {
                    return BadRequest("��lem Ba�ar�s�z.");
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


                }

                var user = _userService.GetById(Guid.Parse(currentUserId));
                user.ProfileImage = uploadResult.Uri.ToString();
                _userService.Update(user);
                return Ok("Cover G�ncellendi.");
            }
            else
            {
                return BadRequest("��lem Ba�ar�s�z.");
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
                    var response = await _userService.ChangePassword(changePasswordViewModel,Guid.Parse(currentUserId));
                    if (response)
                    {
                        return Ok("�ifre De�i�tirildi.");
                    }
                    ModelState.AddModelError("ChangePasswordError","�ifre De�i�tirilemedi.");
                    return BadRequest(ModelState);
                }
                catch (Exception x)
                {
                    ModelState.AddModelError("ChangePasswordError", x.ToString()); // TODO: Replace x with your error message
                    return BadRequest(ModelState);
                }
            }

            return BadRequest("�ifreler Farkl�");
        }






        [HttpGet("changeprofileget")]
        [Authorize]
        public IActionResult ChangeProfileGet()
        {
            
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _userService.GetById(Guid.Parse(currentUserId));
                if (user!=null)
                {
               
                 return Ok(new ChangeInformationProfileViewModel
                {
                    NameSurName = user.NameSurName,
                    Mail = user.Mail,
                    UserName = user.UserName
                });

                }

                return BadRequest("Bir hata olu�tu.");
        }

        [HttpPost("changeprofile")]
        [Authorize]
        public  IActionResult ChangeProfile(ChangeInformationProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _userService.GetById(Guid.Parse(currentUserId));
                user.Mail = model.Mail;
                user.NameSurName = model.NameSurName;
                user.UserName = model.UserName;
                _userService.Update(user);
                return Ok("G�ncelleme Ba�ar�l�");


            }

            return BadRequest("Bir hata olu�tu.");
        }




        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.Get(i=>i.Mail==model.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
               
                //    return Ok();
                //}

              
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return Ok();
        }


        // //
        // // GET: /Account/ResetPassword
        // [HttpGet]
        // [AllowAnonymous]
        // public IActionResult ResetPassword(string code = null)
        // {
        //     return code == null ? View("Error") : View();
        // }



       // [HttpPost]
       //[AllowAnonymous]
       //[ValidateAntiForgeryToken]
       // public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
       // {
       //     if (!ModelState.IsValid)
       //     {
       //         return Ok();
       //     }
       //     var user = _userService.Get(i => i.Mail == model.Email);
       //     if (user == null)
       //     {
               
       //        // return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
       //     }
       //     var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
       //     if (result.Succeeded)
       //     {
       //         return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
       //     }
       //     AddErrors(result);
       //     return View();
       // }


    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.Services.RepositoryServices;
using Chefbook.Model;
using Chefbook.Model.DTO;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


        public UserController(IUserService userService, IConfiguration configuration, IFollowService followService, IPostService postService, IImageService imageService)
        {
            _userService = userService;
            _configuration = configuration;
            _followService = followService;
            _postService = postService;
            _imageService = imageService;
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
                if (kullanicilar!=null)
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
                profileDto.Cover= "https://i.nefisyemektarifleri.com/2018/05/04/mercimek-corbasi-tarifi.jpg";
                profileDto.Description = "Veritabanýna Eklenecek";
                profileDto.FullName = kullanici.NameSurName.Trim();
                profileDto.FollowerCount = followersCount;
                profileDto.PostCount = posts.Count;
                profileDto.ProfilePicture= "https://i.nefisyemektarifleri.com/2018/05/04/mercimek-corbasi-tarifi.jpg";
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
            
          return Ok(walluser);
        }

        //[HttpGet("changepassword")]
        //public IActionResult ChangePassword()
        //{
        //    return 
        //}

        [HttpPost("changepasspost")]
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
                        return Ok("Þifre Deðiþtirildi.");
                    }
                    ModelState.AddModelError("ChangePasswordError","Þifre Deðiþtirilemedi.");
                    return BadRequest(ModelState);
                }
                catch (Exception x)
                {
                    ModelState.AddModelError("ChangePasswordError", x.ToString()); // TODO: Replace x with your error message
                    return BadRequest(ModelState);
                }
            }

            return BadRequest("Þifreler Farklý");
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

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.EntityFrameworkCore;


namespace Chefbook.API.Services.RepositoryServices
{
    public class UserService:GenericRepository<User,ChefContext>,IUserService
    {
        public bool Exists(Guid userId)
        {
            using (var context = new ChefContext())
            {
                return context.User.Any(p => p.Id==userId);
            }
        }

        public async Task<User> Login(string email, string password)
        {
            using (var _context = new ChefContext())
            {

                var user = await _context.User.FirstOrDefaultAsync(x => x.Mail == email || x.UserName==email);
                if (user == null)
                {
                    return null;
                }
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    return null;
                }

                return user;
            }
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            using (var _context = new ChefContext())
            {
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
        }
        public async Task<bool> UserExists(string username)
        {
            using (var _context = new ChefContext())
            {
                if (await _context.User.AllAsync(x => x.UserName == username))
                {
                    return true;
                }
                return false;
            }

        }
        public async Task<bool> MailExists(string mail)
        {
            using (var _context = new ChefContext())
            {
                if (await _context.User.AllAsync(x => x.Mail == mail))
                {
                    return true;
                }
                return false;
            }

        }

        public List<SearchUserModel> Users(string user)
        {
            using (var context= new ChefContext())
            {
               
                var arama= context.User.Where(i => i.UserName.Contains(user)).Select(i => new SearchUserModel()
                {
                    NameSurName = i.NameSurName,
                    ProfileImage = i.ProfileImage,
                    UserName = i.UserName

                });
                return arama.ToList();
            }
        }

        public List<WallPostViewModel> Wall(Guid userId)
        {
            using (var context=new ChefContext())
            {

               
         
                var outputList = from p in context.Post
                                 join u in context.User on p.UserId equals u.Id
                                 join fo in context.Follow on u.Id equals userId
                                 join s in context.Star on p.Id equals s.PostId
                                 
                                 

                                 select new WallPostViewModel
                                 {
                                    
                                     NameSurName = u.NameSurName.Trim(),
                                     PostId = p.Id,
                                     Description = u.Description.Trim(),
                                     LikeCount = p.LikeCount,
                                     PostDate = p.PostDate,
                                     ProfileImage = u.ProfileImage,
                                     Title = p.Title.Trim(),
                                     UserName = u.UserName,
                                     Star = p.Star

                                 };


                return outputList.ToList();
            }

            
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }

                }
                return true;
            }
        }

  

        public async Task<bool> ChangePassword(ChangePasswordViewModel model, Guid userId)
        {

            var user = GetById(userId);
            bool oldpw = VerifyPasswordHash(model.OldPassword, user.PasswordHash, user.PasswordSalt);

            if (model.Password==model.VerifiedPassword && oldpw)
            {
               
                    
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    Update(user);
                    return true;


            }

            return false;

        }

    }
}

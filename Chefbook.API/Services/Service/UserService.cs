using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
                    return false;
                }
                return true;
            }

        }
        public async Task<bool> MailExists(string mail)
        {
            using (var _context = new ChefContext())
            {
                if (await _context.User.AllAsync(x => x.Mail == mail))
                {
                    return false;
                }
                return true;
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

        public List<ExplorePostViewModel> Explore(Guid? userId)
        {

            using (var context=new ChefContext())
            {
                //var begendiklerim = (from p in context.Post
                //    join s in context.Sticker on p.Id equals s.PostId 
                //    join u in context.User on p.UserId equals u.Id
                //    join l in context.Like on p.Id equals l.Id
                //    join f in context.Follow on p.Id equals f.FollowingId
                //    where f.FollowersId == userId

                //    select new ExplorePostViewModel
                //    {

                //        NameSurName = u.NameSurName.Trim(),
                //        PostId = p.Id,
                //        Description = u.Description.Trim(),
                //        LikeCount = p.LikeCount,
                //        PostDate = p.PostDate,
                //        ProfileImage = u.ProfileImage,
                //        Title = p.Title.Trim(),
                //        UserName = u.UserName,
                //        Star = p.Star,
                //        Sticker=s.Name

                //    }).ToList();
                //////////////////////////////////////////////////////////////////////////////////////
                //var kesfet = (from p in context.Post
                //    join s in context.Sticker on p.Id equals s.PostId
                //    join u in context.User on p.UserId equals u.Id
                //    join l in context.Like on p.Id equals l.Id
                //    join f in context.Follow on p.Id equals f.FollowingId
                //    where f.FollowersId != userId

                //              select new ExplorePostViewModel
                //    {

                //        NameSurName = u.NameSurName.Trim(),
                //        PostId = p.Id,
                //        Description = u.Description.Trim(),
                //        LikeCount = p.LikeCount,
                //        PostDate = p.PostDate,
                //        ProfileImage = u.ProfileImage,
                //        Title = p.Title.Trim(),
                //        UserName = u.UserName,
                //        Star = p.Star,
                //        Sticker = s.Name

                //    }).ToList();
                //List<ExplorePostViewModel> explore=new List<ExplorePostViewModel>();
                //foreach (var b in begendiklerim)
                //{
                //     explore.AddRange(kesfet.Where(i=>i.Sticker.Contains(b.Sticker)));

                //}
                var kesfet =(
                    from p in context.Post
                    join u in context.User on p.Id equals u.Id 
                    where (from o in context.Post
                            select o.Id)
                        .Contains(p.UserId)
                    select new ExplorePostViewModel
                    {

                        NameSurName = u.NameSurName.Trim(),
                        PostId = p.Id,
                        Description = u.Description.Trim(),
                        LikeCount = p.LikeCount,
                        PostDate = p.PostDate,
                        ProfileImage = u.ProfileImage,
                        Title = p.Title.Trim(),
                        UserName = u.UserName,
                        Star = p.Star,
                        //Sticker = s.Name

                    }).ToList();




                return null;

            }
           
        }
        public List<WallPostViewModel> Wall(Guid userId)
        {
            using (var context=new ChefContext())
            {



             
                var takipettiklerim = context.Follow.Where(i => i.FollowingId == userId).Select(f=>f.FollowersId);
                List<Post> takipettikleriminpostlari =new List<Post>();
                foreach (var guid in takipettiklerim)
                {
                     takipettikleriminpostlari.AddRange( context.Post.Where(i => i.UserId == guid)); 
                }

                var kendipostlarim = context.Post.Where(i => i.UserId == userId);
                takipettikleriminpostlari.AddRange(kendipostlarim);
                takipettikleriminpostlari = takipettikleriminpostlari.OrderByDescending(i => i.PostDate).ToList();

                List<WallPostViewModel> duvarpostlarim=new List<WallPostViewModel>();
                foreach (var post in takipettikleriminpostlari)
                {
                    DateTime tarih = Convert.ToDateTime(post.PostDate);
                    TimeSpan aralik = DateTime.Now - tarih;

                    if (aralik.Minutes<=1)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(aralik.Seconds);
                        builder.Append(" saniye önce");
                        var kullanici = context.User.FirstOrDefault(i => i.Id == post.UserId);
                        duvarpostlarim.Add(new WallPostViewModel
                        {
                            PostId = post.Id,
                            Star = post.Star,
                            Description = post.Description,
                            LikeCount = post.LikeCount,
                            UserName = kullanici.UserName,
                            NameSurName = kullanici.NameSurName,
                            Title = post.Title,
                            ProfileImage = kullanici.ProfileImage,
                            PostImage = context.Image.Where(i => i.PostId == post.Id).Select(s => s.ImageUrls).ToList(),
                            PostDate = builder.ToString(),

                        });
                    }

                    else if (aralik.Hours<=1)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(aralik.Minutes);
                        builder.Append(" dakika önce");
                        var kullanici = context.User.FirstOrDefault(i => i.Id == post.UserId);
                        duvarpostlarim.Add(new WallPostViewModel
                        {
                            PostId = post.Id,
                            Star = post.Star,
                            Description = post.Description,
                            LikeCount = post.LikeCount,
                            UserName = kullanici.UserName,
                            NameSurName = kullanici.NameSurName,
                            Title = post.Title,
                            ProfileImage = kullanici.ProfileImage,
                            PostImage = context.Image.Where(i => i.PostId == post.Id).Select(s => s.ImageUrls).ToList(),
                            PostDate = builder.ToString(),

                        });
                    }

                   else if (aralik.Days <= 1)
                    {

                        
                        StringBuilder builder = new StringBuilder();
                        builder.Append(aralik.Hours);
                        builder.Append(" saat önce");
                        var kullanici = context.User.FirstOrDefault(i => i.Id == post.UserId);
                        duvarpostlarim.Add(new WallPostViewModel
                        {
                            PostId = post.Id,
                            Star = post.Star,
                            Description = post.Description,
                            LikeCount = post.LikeCount,
                            UserName = kullanici.UserName,
                            NameSurName = kullanici.NameSurName,
                            Title = post.Title,
                            ProfileImage = kullanici.ProfileImage,
                            PostImage = context.Image.Where(i => i.PostId == post.Id).Select(s => s.ImageUrls).ToList(),
                            PostDate = builder.ToString(),

                        });

                    }

                    else if (aralik.Days > 1 || aralik.Days < 30)
                    {
                       // string saat  = aralik.Days + " gün";
                        StringBuilder builder = new StringBuilder();
                        builder.Append(aralik.Days);
                        builder.Append(" gün önce");
                        var kullanici = context.User.FirstOrDefault(i => i.Id == post.UserId);
                        duvarpostlarim.Add(new WallPostViewModel
                        {
                            PostId = post.Id,
                            Star = post.Star,
                            Description = post.Description.Trim(),
                            LikeCount = post.LikeCount,
                            UserName = kullanici.UserName,
                            NameSurName = kullanici.NameSurName,
                            Title = post.Title.Trim(),
                            ProfileImage = kullanici.ProfileImage,
                            PostImage = context.Image.Where(i => i.PostId == post.Id).Select(s => s.ImageUrls).ToList(),
                            PostDate = builder.ToString(),

                        });
                        
                    }
                    else
                    {
                        string saat = tarih.ToLongDateString();

                        var kullanici = context.User.FirstOrDefault(i => i.Id == post.UserId);
                        duvarpostlarim.Add(new WallPostViewModel
                        {
                            PostId = post.Id,
                            Star = post.Star,
                            Description = post.Description,
                            LikeCount = post.LikeCount,
                            UserName = kullanici.UserName,
                            NameSurName = kullanici.NameSurName,
                            Title = post.Title,
                            ProfileImage = kullanici.ProfileImage,
                            PostImage = context.Image.Where(i => i.PostId == post.Id).Select(s => s.ImageUrls).ToList(),
                            PostDate = post.PostDate.ToString(),

                        });
                    }

                   
                }




                ////< 24 hours "x saat"
                ////< 30 gün "x gün önce"
                ////> 30 gün "30 Mayýs 2019
                //List <WallPostViewModel> walllist=new List<WallPostViewModel>();
                //foreach (var model in outputList.ToList())
                //{
                   
                   

                   

                   
                //}


                return duvarpostlarim;
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

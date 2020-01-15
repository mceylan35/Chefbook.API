using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Service
{
    public class PostService : GenericRepository<Post, ChefContext>, IPostService
    {
        public bool Like(Guid postId)
        {
            using (var context=new ChefContext())
            {
                
                if (Exists(postId))
                {
                 var like = context.Post.Find(postId);

                like.LikeCount = like.LikeCount + 1;
                context.SaveChanges();
                return true;
                }

                return false;
            }
        }

        public bool Exists(Guid postId)
        {
            using (var context=new ChefContext())
            {
                return context.Post.Any(p => p.Id == postId);
            }
        }

        public bool DisLike(Guid postId)
        {
            using (var context = new ChefContext())
            {

                if (Exists(postId))
                {


                    var like = context.Post.Find(postId);

                    like.LikeCount = like.LikeCount - 1;
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public IcerikDetayViewModel Detay(Guid postId)
        {
            using (var context=new ChefContext())
            {
                var postbilgi = context.Post.FirstOrDefault(i => i.Id == postId);
                var resimler = context.Image.Where(i => i.PostId == postId).Select(i=>i.ImageUrls).ToList();
                var icindekiler = context.Ingredients.Where(i => i.PostId == postId).Select(i=>i.Ingredient).ToList();
                var adimlar = context.Step.Where(i => i.PostId == postId).Select(i=>i.Description).ToList();
               
                var yorumlar = from c in context.Comment
                    join u in context.User on c.UserId equals u.Id
                    where c.PostId==postId
                    select new CommentUserViewModel
                    {
                        UserName = u.UserName,
                        ProfilePicture = u.ProfileImage,
                        Aciklama = c.Description,
                        LikeCount = c.LikeCount
                    };

                return new IcerikDetayViewModel()
                {
                    Comments = yorumlar.ToList(),
                    Adimlar = adimlar,
                    Icindekiler = icindekiler,
                    Images = resimler,
                    Post = postbilgi
                };
            }
           
        }
    }
}

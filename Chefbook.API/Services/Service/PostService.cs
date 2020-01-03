using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
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

    
    }
}

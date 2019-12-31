using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Service
{
    public class LikeService : GenericRepository<Like, ChefContext>, ILikeService
    {
        public bool BegendinMi(Guid userId, Guid postId)
        {
            using (var context = new ChefContext())
            {
                var begendinmi = context.Like.FirstOrDefault(i => i.BegenenId == userId && i.PostId == postId);
                if (begendinmi!=null)
                {
                   return true;
                }

                return false;
            }
        }

        
    }

    
}

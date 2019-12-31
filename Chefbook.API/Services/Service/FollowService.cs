using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Chefbook.API.Services.Service
{
    public class FollowService : GenericRepository<Follow, ChefContext>, IFollowService
    {
        public bool AmIFollowing(Guid followersId, Guid followingId)
        {
            using (var context=new ChefContext())
            {
                var follow =context.Follow.FirstOrDefault(i =>
                    i.FollowersId == followersId && i.FollowingId == followingId);
                if (follow==null)
                {
                    return false;
                }

                return true;
            }
        }

        public long GetFollowersCount(Guid followedUserId)
        {
            using (var context=new ChefContext())
            {
                long count = context.Follow.Count(i=>i.FollowingId==followedUserId);
                return count;
            }
            
        }

        public long GetFollowingCount(Guid followingUserId)
        {
            using (var context = new ChefContext())
            {
                long count = context.Follow.Count(i=>i.FollowersId == followingUserId);
                return count;
            }
        }

        public bool IsFollowingUser(Guid followerUserId, Guid followedUserId)
        {
            using (var context=new ChefContext())
            {
                var isfollow = context.Follow.FirstOrDefault(i =>
                    i.FollowersId == followedUserId && i.FollowingId == followerUserId);
         

             
                return isfollow != null;
            }
        }

        public List<GetFollowers> GetFollowersUser(Guid followedUserId, int skip = 0, int count = 20)
        {
            using (var context = new ChefContext())
            {
                var users = (from u in context.User
                    join f in context.Follow on u.Id equals f.FollowersId
                    
                   where f.FollowersId== followedUserId
                             select new GetFollowers
                    {
                        Name = u.UserName
                        

                    }).Take(count).Skip(skip).ToList();
                return users;
            }
            
        }

       

        public class GetFollowers 
        {
            public string Name { get; set; }
        }
    }
}

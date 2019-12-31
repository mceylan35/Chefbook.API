using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Repository;
using Chefbook.API.Services.Service;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Interface
{
    public interface IFollowService : IGenericRepository<Follow>
    {
        bool AmIFollowing(Guid followerdId, Guid followingId);
        long GetFollowingCount(Guid followingUserId);
        long GetFollowersCount(Guid followedUserId);
        bool IsFollowingUser(Guid followerUserId, Guid followedUserId);
        List<FollowService.GetFollowers>  GetFollowersUser(Guid followedUserId, int skip = 0, int count = 20);
    }
}

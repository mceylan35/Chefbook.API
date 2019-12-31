using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class UserFollowDTO
    {
        public Guid FollowingId { get; set; }
        public Guid FollowersId { get; set; }
    }
}

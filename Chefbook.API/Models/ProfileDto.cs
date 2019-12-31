using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Chefbook.API.Models
{
    public class ProfileDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public long FollowerCount { get; set; }
        public long PostCount { get; set; }
        public string Cover { get; set; }
        public string ProfilePicture { get; set; }
        public List<ProfilePostsDto> ProfilePosts { get; set; }
      
    }
}

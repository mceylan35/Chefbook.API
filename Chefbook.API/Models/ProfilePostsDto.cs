using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class ProfilePostsDto
    {
        
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CommentCount { get; set; }
        public string LikeCount { get; set; }
        public string RateNumber { get; set; }
        public string PictureUrl { get; set; }
       
    }
}

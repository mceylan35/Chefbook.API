using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class CommentUserViewModel
    {
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
        public string Aciklama { get; set; }
        public long? LikeCount { get; set; }
      
    }

}

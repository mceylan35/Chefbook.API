using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class PostCommentAddDTO
    {
        public Guid PostId { get; set; }
        public string Description { get; set; }
        
    }
}

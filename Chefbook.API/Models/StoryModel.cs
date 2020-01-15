using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class StoryModel
    {
        public Guid StoryId { get; set; }
      
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StoryUrl { get; set; }
        

    }
}

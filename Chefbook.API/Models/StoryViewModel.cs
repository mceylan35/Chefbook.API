using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.Model.Models;

namespace Chefbook.API.Models
{
    public class StoryViewModel
    {
        public StoryViewModel()
        {
            //Stories=new List<StoryModel>();
        }
        public string UserName { get; set; }
        public string UserProfilePicture { get; set; }
        public List<StoryMap> Stories { get; set; }

    }
}

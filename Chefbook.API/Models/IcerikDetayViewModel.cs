using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.Model.Models;

namespace Chefbook.API.Models
{
    public class IcerikDetayViewModel
    {
        public List<string> Icindekiler { get; set; }
        public List<string> Adimlar { get; set; }
        public List<string> Images { get; set; }
        public Post Post { get; set; }
        public List<CommentUserViewModel> Comments { get; set; }

    }
}

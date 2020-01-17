using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class TestWall
    {
        public Guid PostId { get; set; }
        public string NameSurName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string PostDate { get; set; }
        public string Description { get; set; }
        public long? LikeCount { get; set; }
        public string Title { get; set; }
        public double Star { get; set; }
        public List<string> PostImage { get; set; }
    }
}

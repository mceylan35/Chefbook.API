using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class StoryMap
    {
        public Guid StoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StoryUrl { get; set; }

    }
}

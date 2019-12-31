using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public Guid? PostId { get; set; }
        public string ImageUrls { get; set; }
        public string PublicId { get; set; }
    }
}

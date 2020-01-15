using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Chefbook.API.Models
{
    public class StoryDto
    {

        public List<IFormFile> Stories { get; set; }

    }
}

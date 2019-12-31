using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Http;

namespace Chefbook.API.Models
{
    public class PostDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<StepDTO> Steps { get; set; }
        public List<IFormFile> Photos { get; set; }
       
      
    }
}

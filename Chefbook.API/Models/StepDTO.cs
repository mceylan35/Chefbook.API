using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class StepDTO
    {
        

        public Guid? PostId { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}

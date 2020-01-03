using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class StarDto
    {
        public Guid PostId { get; set; }
        public byte RateNumber { get; set; }
       
    }
}

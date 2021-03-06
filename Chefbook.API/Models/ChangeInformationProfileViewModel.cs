﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class ChangeInformationProfileViewModel
    {
       
        public string NameSurName { get; set; }
        [Required]
        public string Mail { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Biography { get; set; }
    }
}

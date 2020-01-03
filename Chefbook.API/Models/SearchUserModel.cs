using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class SearchUserModel
    {
        public string NameSurName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }

    }
}

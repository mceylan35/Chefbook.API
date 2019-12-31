using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chefbook.Model.DTO
{
    public class RegisterDTO
    {

        public string NameSurName { get; set; }

        public string UserName { get; set; }
        //public bool? Gender { get; set; }
       // [Column(TypeName = "date")]
       // public DateTime? Birthday { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }



    }
}

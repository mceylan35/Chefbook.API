using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Models
{
    public class ChangePasswordViewModel
    {

        [Required(ErrorMessage = "Old Password Zorunlu")]
        [StringLength(255, ErrorMessage = "Password En az 6 en fazla 16 karakter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Password Zorunlu")]
        [StringLength(255, ErrorMessage = "Password En az 6 en fazla 16 karakter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Password Zorunlu")]
        [StringLength(255, ErrorMessage = "Password En az 6 en fazla 16 karakter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "İkiside Aynı olmalı")]
        public string VerifiedPassword { get; set; }
    }
}

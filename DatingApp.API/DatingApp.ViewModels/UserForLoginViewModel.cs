using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatingApp.ViewModels
{
   public class UserForLoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify password b/w 4 and 8 characters")]
        public string Password { get; set; }
    }
}

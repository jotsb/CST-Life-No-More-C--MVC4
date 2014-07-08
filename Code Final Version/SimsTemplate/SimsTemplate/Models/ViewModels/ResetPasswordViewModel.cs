using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SimsTemplate.Models.ViewModels
{

    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "You forgot to enter your new password.")]
        [Display(Name = "New Password:")]
        public string Password { get; set; }

        
        [Required(ErrorMessage = "You forgot to enter your confirmation password.")]
        [Display(Name = "Confirmation Password:")]
        public string ConfPassword { get; set; }

    }

}
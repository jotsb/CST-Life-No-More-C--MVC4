using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SimsTemplate.Models.ViewModels
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// @Date: Nov 1, 2012
    /// </summary>
    public class LoginViewModel
    {

        //email_formatted
        [Required(ErrorMessage = "You forgot to enter your email address.")]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        //password
        [Required(ErrorMessage = "You forgot to enter your password.")]
        [Display(Name = "Password:")]
        public string Password { get; set; }

    }
}
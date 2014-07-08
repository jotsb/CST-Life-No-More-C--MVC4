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
    public class RegisterNewUserViewModel
    {

        //data for a new user
        /// <summary>
        /// The user's first name
        /// This properties is required and displays an error message if not filled in
        /// it allows for 1-160 alpha numberic characters, spaces and apostrophes
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression("^[a-zA-Z\\'\\s]{1,120}$", ErrorMessage = "Can only be characters")]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You forgot your username.")]
        [MinLength(3)]
        [Display(Name = "Username:")]
        public string Username { get; set; }

        /// <summary>
        /// The user's last name
        /// This properties is required and displays an error message if not filled in
        /// it allows for 1-160 alpha numberic characters, spaces and apostrophes
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression("^[a-zA-Z\\'\\s]{1,120}$", ErrorMessage = "Can only be characters")]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        //email_formatted
        [Required(ErrorMessage = "You forgot you email.")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Can only be in email format. e.g. email@email.com")]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// This properties is required and displays an error message if not filled in
        /// Allows 1-160 anything thing as a password
        /// </summary>
        [Required(ErrorMessage = "A password is required.")]
        [MinLength(1)]
        [MaxLength(160)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The user's password confirmation
        /// This properties is required and displays an error message if not filled in
        /// Allows 1-160 anything thing as a password
        /// </summary>
        [Required(ErrorMessage = "A confirmation password is required.")]
        [MinLength(1)]
        [MaxLength(160)]
        [DataType(DataType.Password)]
        public string ConfirmationPassword { get; set; }

        /// <summary>
        /// Age of the user (an integer). Not required but desirable.
        /// </summary>
        public Nullable<int> Age { get; set; }



    }





}
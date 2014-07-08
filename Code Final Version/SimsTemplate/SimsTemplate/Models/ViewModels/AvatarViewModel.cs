using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SimsTemplate.Models.ViewModels
{
    /// <summary>
    /// View model for the avatar image url.
    /// </summary>
    public class AvatarViewModel
    {
        //[UIHint("AvatarImage")]
        public string ImageUrl { get; set; }
    }
}
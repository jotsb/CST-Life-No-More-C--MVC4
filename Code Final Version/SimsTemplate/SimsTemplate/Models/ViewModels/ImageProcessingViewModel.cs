using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SimsTemplate.Models.ViewModels
{
    public class ImageProcessingViewModel
    {
    }

    public class ProfileViewModel
    {
        [UIHint("ProfileImage")]
        public string ImageUrl { get; set; }
    }

    public class EditorInputModel
    {
        public ProfileViewModel Profile { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }


}
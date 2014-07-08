using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimsTemplate.Models.ViewModels
{

    /// <summary>
    /// View Model for editing the avatar photo.
    /// </summary>
    public class AvatarEditorModel
    {
        public AvatarViewModel Avatar { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
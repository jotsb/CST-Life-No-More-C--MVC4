using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SimsTemplate.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        // SelectBox
        [Required(ErrorMessage = "")]
        [Display(Name = "Sorting List:")]
        public List<SelectListItem> ListItems { get; set; }

        // Item selected in the SelectBox
        public Int32 selectedItem { get; set; }

    }
}
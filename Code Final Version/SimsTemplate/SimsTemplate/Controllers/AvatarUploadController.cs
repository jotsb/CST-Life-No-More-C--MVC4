using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Helpers;
using SimsTemplate.Models.ViewModels;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AvatarUploadController : Controller
    {


        /// <summary>
        /// GET: /AvatarUpload/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ProfileViewModel pmodel = new ProfileViewModel();

            return View("AvatarUpload", pmodel);
        }


        /// <summary>
        /// POST: /AvatarUpload/
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateAvatar(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }

            return RedirectToAction("Log_in","Login");
        }

    }
}

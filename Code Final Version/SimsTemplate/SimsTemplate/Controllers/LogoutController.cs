using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Models.ViewModels;
using SimsTemplate.Helper;
using System.Net.Mail;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// Controller to handle user logout.
    /// Clears the session or cookies if applicable.
    /// </summary>
    public class LogoutController : Controller
    {
       
        /// <summary>
        /// GET: /Logout/
        /// @Author: Aubrey Fowler
        /// @Date: Oct 27, 2012
        /// Clears the session data and returns the user to the home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //if it exists, delete it
            if (Request.Cookies["UserInfo"] != null)
            {
                //delete the cookie
                HttpCookie aCookie = new HttpCookie("UserInfo");
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            } //else just ignore it

            //clear the session
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index" , "Home");
        }

    }
}

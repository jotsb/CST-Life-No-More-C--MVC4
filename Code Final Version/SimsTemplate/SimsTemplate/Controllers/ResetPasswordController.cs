using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Helper;
using SimsTemplate.Models.ViewModels;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// Controller that handles password resets.
    /// </summary>
    public class ResetPasswordController : Controller
    {

        /// <summary>
        /// Database connection.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Aubrey Fowler
        /// Return the view so the user can reset their password
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string user_email)
        {
            ViewBag.Message = user_email;
            //just display the form
            return View("ResetPassword");
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// Collect the data from the view and send the user to the login page if the
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(ResetPasswordViewModel rp)
        {

            //get the email from the URL
            string email = Request.QueryString["user_email"];

            //collect the data from the form
            if (ModelState.IsValid)
            {

                if (!SecurityHelper.StripHTML(rp.Password).Equals(SecurityHelper.StripHTML(rp.ConfPassword))) //the passwords have to match
                {
                    ViewBag.Message = "The two passwords have to match.";
                    return View("ResetPassword");
                }
                else //made it through both checks
                {

                    var collectUser = (
                        from usr in db.Users
                        where usr.email.Equals(SecurityHelper.StripHTML(email))
                        select usr
                        ).First();

                    collectUser.password = AuthenticationHelper.ENCRYPT_ME(SecurityHelper.StripHTML(rp.Password));

                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index", "Login");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Something weird happened. Try again.";
                        return View("ResetPassword");
                    }

                }


            }
            else
            {
                ViewBag.Message = "You have to fill in both fields.";
                return View("ResetPassword");
            }

        }















    }
}

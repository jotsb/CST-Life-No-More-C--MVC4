using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Models.ViewModels;
using SimsTemplate.Helper;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// Controller to handler user login and authentication.
    /// </summary>
    public class LoginController : Controller
    {

        /// <summary>
        /// Connnect to the database.
        /// </summary>
        TechproContext db = new TechproContext();
        
        /// <summary>
        /// @Author: Aubrey Fowler
        /// GET :/Index/
        /// Presents the login form to the users.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("LoginView");
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// POST :/Login/
        /// Validate and submits the login form called: LoginView
        /// Makes use fo the LoginViewModel
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(LoginViewModel lvm, FormCollection fm)
        {

            try
            {

                //strip the html
                string temp = SecurityHelper.StripHTML(lvm.Email);
                string pw = SecurityHelper.StripHTML(lvm.Password);

                var userLogin = (from user in db.Users
                                 where user.email.Equals(temp)
                                 select user).SingleOrDefault();

                if(userLogin == null)
                {
                    ViewBag.Message = "You are not in the database. Please register.";
                    return View("LoginView");
                }

                if (userLogin != null && userLogin.password.Equals(AuthenticationHelper.ENCRYPT_ME(pw)))
                {

                    //set the session variables
                    SessionHandler.Logon = true;
                    SessionHandler.Role = userLogin.role;
                    SessionHandler.UID = userLogin.id;

                    //if the box is checked, create the cookie
                    if (fm.Get("remember").Contains("true") == true) {

                        //needs to be created - check if the cookie exists
                        if (Request.Cookies["UserInfo"] == null) {

                            //create the cookie
                            HttpCookie _userInfoCookies = new HttpCookie("UserInfo");

                            //add the data to the cookie
                            //_userInfoCookies.Value = "yadda";
                            _userInfoCookies["username"] = userLogin.username;
                            _userInfoCookies["uid"] = userLogin.id.ToString();
                            _userInfoCookies["role"] = userLogin.role;
                            ////add the expiry date - persistent cookie
                            _userInfoCookies.Expires = DateTime.MaxValue;
                            //add the cookie to the current response

                            Response.Cookies.Add(_userInfoCookies);
                            Response.SetCookie(_userInfoCookies);
                                                        
                        }
                        //else do nothing the cookie already exists
                    }

                    //update the hearbeat montior
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

                    //go to your profile page
                    return RedirectToAction("Index", "Profile");

                }
                else
                {
                    ViewBag.Message = "Your password is wrong.";
                    return View("LoginView");
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = "You are not in the database. Please register.";
                return View("LoginView");
            }


        }


    }
}

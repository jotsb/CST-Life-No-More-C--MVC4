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
    /// @Date: October 25, 2012
    /// Controller to handle user registraton.
    /// </summary>
    public class UserController : Controller
    {

        /// <summary>
        /// Connect to the database.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Aubrey Fowler
        /// ActionResult to direct the user to the New User Registration View.
        /// GET: /User/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("NewUserRegistrationView");
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// What happends after the new user registration form is submitted. 
        /// POST to submit the data into the
        /// database and register the new user on the web site.
        /// POST: /User/
        /// Checks to see if the user is already in the database.
        /// Validation is also supplied by this feature.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection formCollection, RegisterNewUserViewModel registration_view_model_data)
        {

            //if the form is valid
            if (ModelState.IsValid)
            {

                string temp_email = SecurityHelper.StripHTML(registration_view_model_data.Email);
                string temp_user_name = SecurityHelper.StripHTML(registration_view_model_data.Username);

                //check to see if the user is already in the database
                var user_in_question = (from usr in db.Users
                            where usr.email.Equals(temp_email)
                            select usr);

                //if this is more than zero, it means the user is already in the database
                if (user_in_question.FirstOrDefault() != null)
                {
                    ViewBag.Message = "You are already in the database! Please just login.";
                    return View("NewUserRegistrationView");
                }

                //check to see if the username is unique
                var username_in_question = (from usr in db.Users
                                        where usr.username.Equals(temp_user_name)
                                        select usr);

                //if this is more than zero, it means the user is already in the database
                if (username_in_question.FirstOrDefault() != null)
                {
                    ViewBag.Message = "User name must be unique. This one is already in the database.";
                    return View("NewUserRegistrationView");
                }

                //check if the password matches
                if (!SecurityHelper.StripHTML(registration_view_model_data.ConfirmationPassword).Equals(SecurityHelper.StripHTML(registration_view_model_data.Password)))
                {
                    ViewBag.Message = "The two passwords have to match.";
                    return View("NewUserRegistrationView");
                }

                //create a new user
                User user = new User();
                user.firstname = SecurityHelper.StripHTML(registration_view_model_data.FirstName);
                user.lastname = SecurityHelper.StripHTML(registration_view_model_data.LastName);
                user.datetime_registered = DateTime.Now;
                user.role = "user";
                user.username = temp_user_name;
                user.sex = formCollection["sex"];
                user.location = formCollection["location"];
                user.age = Convert.ToInt32(SecurityHelper.StripHTML(registration_view_model_data.Age.ToString()));
                //encrypt the password
                user.password = AuthenticationHelper.ENCRYPT_ME(SecurityHelper.StripHTML(registration_view_model_data.Password));
                user.email = temp_email;

                user.image_url = formCollection["sex"].Equals("m") ? "~/Content/Images/male_default.jpg" : "~/Content/Images/female_default.jpg"; 

                try
                {

                    //submit into the database
                    db.Users.Add(user);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Login");

                }
                catch (InvalidOperationException ex)
                {
                    ViewBag.Message = "Email is already in the database.";
                    return View("NewUserRegistrationView");
                }
                catch (FormatException ex)
                {
                    ViewBag.Message = "You need to enter a numberical value for your age.";
                    return View("NewUserRegistrationView");
                }
                catch (InvalidCastException ex)
                {
                    ViewBag.Message = "The age conversion totally failed.";
                    return View("NewUserRegistrationView");
                }
                catch (OverflowException)
                {
                    ViewBag.Message = "Your age is way to big. Enter a value between 0 and 120";
                    return View("NewUserRegistrationView");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Something weird happend. Please try again.";
                    //redirect to the registration page again
                    return View("NewUserRegistrationView");
                }

            }
            else
            {
                ViewBag.Message = "Too many errors. Please fill out the form again.";
                return View("NewUserRegistrationView");
            }

        }


        /// <summary>
        /// @Author: Taylor Dixon
        /// @Date: Nov 02, 2012
        /// Redirects to Chat controller
        /// </summary>
        /// <returns></returns>
        public ActionResult Chat()
        {
            return RedirectToAction("Index", "Chat");
        }



    }
}

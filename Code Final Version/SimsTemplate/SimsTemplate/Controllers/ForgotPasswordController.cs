using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Models.ViewModels;
using SimsTemplate.Helper;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SimsTemplate.Controllers
{
 
    /// <summary>
    /// @Author: Aubrey Fowler
    /// Controller to handle the situation where the user forgot their password.
    /// </summary>
    public class ForgotPasswordController : Controller
    {

        /// <summary>
        /// Connect to the database.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Aubrey Fowler
        /// Http: GET
        /// Click on the link if you forgot your password.
        /// Will go to the form to enter your email_formatted address.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //just return the view
            ViewBag.Message = "Please enter your email below:";
            return View("ForgotUserPassword");
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// POST :/ForogtPassword/
        /// Changes and validates the new password.
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string email)
        {

            //if they entered a value
            if (!string.IsNullOrWhiteSpace(email))
            {

                try
                {

                    string formatted = SecurityHelper.StripHTML(email);
                    //check if they are in the database or not
                    var findUsers = (from user in db.Users
                                     where user.email.Equals(formatted)
                                     select user
                                    );

                    //if there is none, it will return null
                    if (findUsers.FirstOrDefault() == null)
                    {
                        //user was not found in the database
                        ViewBag.Message = "You are not in the database. Please register first.";
                        return View("ForgotUserPassword");
                    }
                    else
                    {
                        //user was found in the database
                        ViewBag.Message = "Please check your email. :)";
                        //send an email to the user
                        send_email_to_user(formatted);
                        return View("ForgotPassowrdEmailConfirmation");
                    }
                }
                catch (NullReferenceException ex)
                {
                    ViewBag.Message = "null reference exception " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (ArgumentNullException ex)
                {
                    ViewBag.Message = "Argument null exception " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (SmtpException ex)
                {
                    ViewBag.Message = "SMTPException " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (FormatException ex)
                {
                    ViewBag.Message = "FORMAT " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (InvalidOperationException ex)
                {
                    ViewBag.Message = "Invalid operation exception " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch(ArgumentOutOfRangeException ex)
                {
                    ViewBag.Message = "Argument out of range exception:::. " + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (ArgumentException ex) 
                {
                    ViewBag.Message = "Argument Exception:::." + ex.Message;
                    return View("ForgotUserPassword");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Generic Exception::Something weird happened:::." + ex.Message;
                    return View("ForgotUserPassword");
                }
            }
            else
            {
                ViewBag.Message = "You didn't enter your email.";
                return View("ForgotUserPassword");
            }
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// Send an email_formatted to the specified user so they can reset thier password.
        /// </summary>
        private void send_email_to_user(string user_email)
        {

            MailMessage message = new MailMessage();
            message.To.Add(user_email);
            message.Subject = "Reseting Your Password in CSTSims";
            message.From = new MailAddress("cstsims2012@gmail.com");

            //email_formatted message
            message.Body = "Hello from CSTSims,\n"
                + "\nPlease click on the link below to reset your password: "
                + "http://142.232.17.225/ResetPassword/Index/?user_email=" + user_email
                + "\n\nBest Regards, \nCSTSims Team";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("cstsims2012@gmail.com", "techpro2012"),
                EnableSsl = true
            };

            client.Send(message);

        }


    }
}

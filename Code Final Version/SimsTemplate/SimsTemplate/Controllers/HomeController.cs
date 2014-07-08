using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : Controller
    {

        /// <summary>
        /// @Author: Aubrey, Jot, Shan
        /// Database connection for cookie check.
        /// </summary>
        //TechproContext db = new TechproContext();

        /// <summary>
        /// Stores the Exception value.
        /// </summary>
        static string errorException;

        /// <summary>
        /// @Author: Aubrey, Jot, Shan
        /// Check if the user cookie was set - if it is the user will be logged in.
        /// Display the home page of the web site.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
            //if they arrive at the home page, check if the persistent cookie exists
            /*if (Request.Cookies["UserInfo"] != null)
            {

                int check_id = Convert.ToInt32(Request.Cookies["userInfo"]["uid"]);
                string uname = Request.Cookies["userInfo"]["username"];
                string ro = Request.Cookies["userInfo"]["role"];

                //query the database data based on the cookie data
                var user_in_question = (
                    from usr in db.Users
                    where usr.id == check_id
                    && usr.username.Equals(uname)
                    && usr.role.Equals(ro)
                    select usr
                    );

                //check the result - the person was found in the db - log them in
                if (user_in_question.FirstOrDefault() != null)
                {
                    SessionHandler.Logon = true;
                    SessionHandler.Role = Request.Cookies["userInfo"]["role"];
                    SessionHandler.UID = Convert.ToInt32(Request.Cookies["userInfo"]["uid"]);

                   //set the cookie expiry date - imitate an infinite cookie
                   Response.Cookies["userInfo"].Expires = DateTime.MaxValue;
               }

            } //else just go to the home page*/

            return View();
        }

        /// <summary>
        /// @Author: Aubrey, Jot, Shan
        /// This should be in the profile controller. Thanks, Aubrey.
        /// </summary>
        /// <returns></returns>
        public ActionResult UserProfile()
        {
            return RedirectToAction("Index", "Profile");
        }

        /// <summary>
        /// @Author: Aubrey, Jot, Shan, Patrick
        /// Direct users to the forum feature.
        /// </summary>
        /// <returns></returns>
        public ActionResult Forum()
        {
            return RedirectToAction("Index", "Forum");
        }

        /// <summary>
        /// @Author: Aubrey, Jot, Shan
        /// Direct users to the chat feature.
        /// </summary>
        /// <returns></returns>
        public ActionResult Chat()
        {
            return RedirectToAction("Index", "Chat");
        }

        /// <summary>
        /// Direct users to the contact page.
        /// @Author: Jot, Shan
        /// @author: Jivanjot Brar
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        /// <summary>
        /// Contact Post Method. Takes in the values from the ContactViewModel.
        /// 
        /// @author: Jivanjot Brar
        /// </summary>
        /// <param name="contactVM"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Contact(ContactViewModel contactVM)
        {
            if (!ModelState.IsValid)
            {
                return View(contactVM);
            }

            var contact = new Contact
            {
                Name = contactVM.Name,
                Email = contactVM.Email,
                Subject = contactVM.Subject,
                Comment = contactVM.Comment
            };

            try
            {
                new Email().Send(contact);
                return RedirectToAction("ContactConfirm");
            }
            catch (Exception ex)
            {
                ViewBag.EmailMsg = "Following Error Occured during sending a message: ";
                ViewBag.Error = ex.Message;
                return View("Contact");
            }
        }

        /// <summary>
        /// Action called when there is an error when sending an email.
        /// 
        /// @author: Jivanjot Brar
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactConfirm()
        {
            ViewBag.Msg = "Message has been successfully sent and we will get back to as soon as possible.";
            return View("Contact");
        }

        /// <summary>
        /// Action called when a server error occurs.
        /// 
        /// @Author: Jivanjot Brar & Shan Bains
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }



    }
}

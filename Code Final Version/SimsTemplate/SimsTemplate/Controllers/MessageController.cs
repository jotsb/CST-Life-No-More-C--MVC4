using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Helper;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// @Author: Patrick Tseng
    /// Message Controller
    /// </summary>
    public class MessageController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        private TechproContext db = new TechproContext();

        /// <summary>
        /// Checks if user is alive
        /// </summary>
        HeartbeatMonitor loginTimer;

        /// <summary>
        /// @Author: Patrick Tseng
        /// Returns a list of messages for the user descending from time received
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            //checks if the user has been authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");
            else
                loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            //grabs all the messages
            var usermessages = db.UserMessages
                                    .Include(u => u.User)
                                    .Include(u => u.User1)
                                    .Where(u => u.recipient_id == SessionHandler.UID)
                                    .OrderByDescending(u => u.datetime_sent);
            
            return View(usermessages.ToList());
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Displays the message content
        /// </summary>
        /// <param name="id">message ID</param>
        /// <returns>View</returns>
        public ActionResult View(int id)
        {
            //checks of the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //grabs the message with the message id passed in as the parameter
            var message = db.UserMessages.Include(u => u.User).Include(u => u.User1).Where(u => u.id == id).SingleOrDefault();

            //if message does not exist, throw an exception and redirect
            if (message == null)
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("Message does not exist."));

            //set message status as read
            message.message_read = true;

            db.SaveChanges();

            return View(message);
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Creates a reply
        /// </summary>
        /// <param name="replyPostID">ID of reply, -1 means that the post is not a reply</param>
        /// <returns>View</returns>
        public ActionResult Create(int replyPostID = -1)
        {
            //checks of the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            ViewBag.IsDirectMessage = false;
            ViewBag.sender_id = new SelectList(db.Users, "id", "username");

            if (replyPostID > -1) //if the post is a reply, set the subject and message body to original post
            {
                UserMessage msg = db.UserMessages.SingleOrDefault(i => i.id == replyPostID);
                if (msg != null)
                {
                    ViewBag.recipient_id = new SelectList(db.Users, "id", "username", msg.sender_id);
                    ViewBag.Subject = "Re: " + msg.subject;
                    ViewBag.Message = "\r\n---------------------------------------------\r\n" + msg.message;
                    ViewBag.IsReply = true;
                }
            }
            else //otherwise proceed as normal
            {
                ViewBag.recipient_id = new SelectList(db.Users, "id", "username");
                ViewBag.IsReply = false;
            }

            return View();
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Creates a message to a particular user
        /// </summary>
        /// <param name="username">username of the user to send a message to</param>
        /// <returns>View</returns>
        public ActionResult DirectMessage(string username)
        {
            //checks of the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //if the user does not exist, throw an exception and redirect
            if (db.Users.SingleOrDefault(i => i.username == username) == null)
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User does not exist."));

            //set required fields for views
            ViewBag.IsDirectMessage = true;
            ViewBag.Recipient = username;
            ViewBag.RecipientID = db.Users.SingleOrDefault(i => i.username == username).id;
            ViewBag.IsReply = false;
            
            return View("Create");
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Sends a message
        /// </summary>
        /// <param name="usermessage">Message to send</param>
        /// <returns>Redirect to message index</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(UserMessage usermessage)
        {
            //checks if the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //sets required UserMessage fields and adds the message to the database
            usermessage.message = SecurityHelper.StripHTMLExceptAnchor(usermessage.message); //strip HTML and JS tags
            usermessage.datetime_sent = DateTime.Now;
            usermessage.message_read = false;
            int uid = SessionHandler.UID;
            usermessage.sender_id = SessionHandler.UID;
            db.UserMessages.Add(usermessage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Deletes a message given the ID
        /// </summary>
        /// <param name="id">message ID</param>
        /// <returns>Redirect to message index</returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserMessage usermessage = db.UserMessages.Find(id);
            db.UserMessages.Remove(usermessage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Auto-generated dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


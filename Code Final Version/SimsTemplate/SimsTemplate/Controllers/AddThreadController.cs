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
    /// </summary>
    public class AddThreadController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        private TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Patrick Tseng
        /// Returns a view for adding a new thread
        /// </summary>
        /// <param name="subforumID">ID of parent subforum</param>
        /// <returns>View</returns>
        public ActionResult NewThread(int subforumID = 1)
        {
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            User user = db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            ViewBag.SubforumID = subforumID;
            ViewBag.UserID = SessionHandler.UID;
            ViewBag.SubforumName = db.Subforums.SingleOrDefault(i => i.id == subforumID).title;
            return View();
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Creates a forum thread
        /// </summary>
        /// <param name="subforumID">parent subforum ID</param>
        /// <param name="userID">author ID</param>
        /// <param name="threadTitle">title of thread</param>
        /// <param name="threadContent">content of thread</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int subforumID, int userID, string threadTitle, string threadContent)
        {
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            User user = db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            ForumThread ft = new ForumThread();
            ft.author_id = userID;
            ft.datetime_posted = DateTime.Now;
            ft.num_hits = 1;
            ft.title = SecurityHelper.StripHTML(threadTitle);
            ft.subforum_id = subforumID;
                
            db.ForumThreads.Add(ft);

            db.SaveChanges();
            
            ForumPost fp = new ForumPost();
            fp.author_id = userID;
            fp.datetime_posted = DateTime.Now;
            fp.thread_id = ft.id;
            fp.text = SecurityHelper.StripHTMLExceptAnchor(threadContent);

            db.ForumPosts.Add(fp);

            db.SaveChanges();

            return RedirectToAction("Index", "Posts", new { threadID = ft.id });
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
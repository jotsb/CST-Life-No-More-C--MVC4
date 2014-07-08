using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate;
using System.Collections;
using SimsTemplate.Helper;

namespace Forum.Controllers
{
    /// <summary>
    /// @Author: Patrick Tseng
    /// Post Controller
    /// </summary>
    public class PostsController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        TechproContext _db = new TechproContext();

        /// <summary>
        /// Checks if user is alive
        /// </summary>
        HeartbeatMonitor loginTimer;

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Returns all the posts from a thread
        /// </summary>
        /// <param name="threadID">ID of thread</param>
        /// <returns>View</returns>
        public ActionResult Index(int threadID)
        {
            if (SessionHandler.Logon)
                loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            ForumPost test = _db.ForumPosts.FirstOrDefault(i => i.thread_id == threadID);
            if (test == null)
            {
                return View("Error", new ArgumentException("Thread does not exist!"));
            }

            //gets the thread title and all the posts
            ViewBag.ThreadID = threadID;
            ViewBag.ThreadTitle = _db.ForumThreads.First(i => i.id == threadID).title;
            var posts = _db.ForumPosts.Where(i => i.thread_id == threadID);

            //post author and post author's avatar
            //here because the author_id isn't a foreign key to the users table, oops
            Hashtable postAuthors = new Hashtable();
            Hashtable postAuthorsAvatar = new Hashtable();

            //grabs the author's name and avatar given the ID
            foreach (var post in posts)
            {
                postAuthors.Add(post.id, _db.Users.Single(i => i.id == post.author_id).username);
                postAuthorsAvatar.Add(post.id, _db.Users.Single(i => i.id == post.author_id).image_url);
            }

            var subforumID = _db.ForumThreads.Single(i => i.id == threadID);
            var parentSubforum = _db.Subforums.First(j => j.id == subforumID.subforum_id);

            //sets view parameters
            ViewBag.SubforumName = parentSubforum.title;
            ViewBag.SubforumID = parentSubforum.id;
            ViewBag.PostAuthors = postAuthors;
            ViewBag.PostAuthorsAvatar = postAuthorsAvatar;
            ViewBag.HeadPost = posts.FirstOrDefault().id;

            _db.ForumThreads.SingleOrDefault(i => i.id == threadID).num_hits += 1;

            _db.SaveChanges();

            return View(posts.ToList());
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Creates a new post in a thread
        /// </summary>
        /// <param name="threadID">thread ID of the post</param>
        /// <returns>Partial view</returns>
        public ActionResult Create(int threadID)
        {
            //checks if the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //checks if the user is banned
            User user = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            //grabs relavent thread information
            ViewBag.ParentThread = threadID;
            ViewBag.ThreadTitle = _db.ForumThreads.First(i => i.id == threadID).title;

            return PartialView();
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Adds a post
        /// </summary>
        /// <param name="parentThread">ID of parent thread</param>
        /// <param name="postContent">content to post</param>
        /// <returns>Redirect to the parent thread</returns>
        [HttpPost]
        public ActionResult AddPost(int parentThread, string postContent)
        {
            //checks if the user has been authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //checks if the user is banned
            User user = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            //creates a new forum posts and adds it to the database
            ForumPost fp = new ForumPost();
            fp.thread_id = parentThread;
            fp.author_id = SessionHandler.UID;
            fp.text = SecurityHelper.StripHTMLExceptAnchor(postContent);
            fp.datetime_posted = DateTime.Now;

            _db.ForumPosts.Add(fp);

            _db.SaveChanges();

            return RedirectToAction("Index", new { threadID = parentThread });
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Allows for editing of posts for post author and admins
        /// </summary>
        /// <param name="id">id of post to edit</param>
        /// <returns>View</returns>
        public ActionResult Edit(int id)
        {
            //checks if the user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //checks if the user is banned
            User user = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            ForumPost fp = _db.ForumPosts.SingleOrDefault(i => i.id == id);

            if (fp == null)
            {
                ViewBag.ErrorFlag = true;
                ViewBag.Error = "Post does not exist.";
                return View();
            }

            if (!(SessionHandler.Role == "admin" | SessionHandler.UID == fp.author_id))
            {
                ViewBag.ErrorFlag = true;
                ViewBag.Error = "You do not have permissions to do this.";
                return View();
            }

            ViewBag.ErrorFlag = false;
            ViewBag.PostID = id;
            ViewBag.PostContent = SecurityHelper.StripHTMLExceptAnchor(fp.text);

            return View();
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Submits the edit to the database
        /// </summary>
        /// <param name="postID">ID of post to edit</param>
        /// <param name="postContent">edited content</param>
        /// <returns>Redirect to thread</returns>
        [HttpPost]
        public ActionResult Edit(int postID, string postContent)
        {
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //checks if the user is banned
            User user = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));
            
            ForumPost fp = _db.ForumPosts.SingleOrDefault(i => i.id == postID);

            fp.text = postContent;

            _db.SaveChanges();

            return RedirectToAction("Index", new { threadID = fp.thread_id });
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Allows deletion of posts from post author and admins
        /// </summary>
        /// <param name="id">post ID to delete</param>
        /// <returns>View</returns>
        public ActionResult Delete(int id)
        {
            //checks of user is authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //checks if the user is banned
            User user = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID);

            if (user.is_banned)
                return View("Error", new ArgumentException("You cannot add new threads or posts while banned."));

            ForumPost fp = _db.ForumPosts.SingleOrDefault(i => i.id == id);

            //if the post doesn't exist, redirect, or if not the post author or admin
            if (fp == null)
                ViewBag.Error = "Selected post does not exist.";
            else if (!(SessionHandler.Role == "admin" | SessionHandler.UID == fp.author_id))
            {
                ViewBag.Error = "Permission denied. You do not have the privileges to do this.";
            }
            else
            {
                int threadID = fp.thread_id;
                _db.ForumPosts.Remove(fp);
                _db.SaveChanges();
                return RedirectToAction("Index", new { threadID = threadID });
            }

            return View();
        }

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Generates messages to admins if a post is reported
        /// </summary>
        /// <param name="id">id of post to report</param>
        /// <returns>Redirects to thread</returns>
        public ActionResult Report(int id)
        {
            //checks if the user has been authenticated
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            //gets the post in question
            ForumPost fp = _db.ForumPosts.SingleOrDefault(i => i.id == id);
            int threadID = 0;

            if(fp != null)
            {
                threadID = fp.thread_id;
                string username = _db.Users.SingleOrDefault(i => i.id == SessionHandler.UID).username;
                var users =     from u in _db.Users
                                where u.role == "admin"
                                select u;

                //generates a message for each of the admins and sends them
                foreach (User admin in users)
                {
                    UserMessage um = new UserMessage();
                    um.datetime_sent = DateTime.Now;
                    um.id = 1;
                    um.sender_id = 69;
                    um.recipient_id = admin.id;
                    um.subject = "Forum post reported";
                    um.message = username + " reported post " + "<a href=\"/Posts?threadID=" + threadID + "#" + id + "\">#" + id + "</a>.";
                    um.message_read = false;
                    _db.UserMessages.Add(um);
                }

                _db.SaveChanges();
            }

            return RedirectToAction("Index", new { threadID = threadID});
        }
    }
}

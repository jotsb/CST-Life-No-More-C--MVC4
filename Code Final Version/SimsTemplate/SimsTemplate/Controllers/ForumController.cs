using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate;
using System.Collections;

namespace Forum.Controllers
{
    /// <summary>
    /// Forum controller
    /// @Author: Patrick Tseng
    /// </summary>
    public class ForumController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        private TechproContext _db;

        /// <summary>
        /// @Author: Patrick Tseng
        /// Checks if user is alive
        /// </summary>
        HeartbeatMonitor loginTimer;

        /// <summary>
        /// @Author: Patrick Tseng
        /// Forum Controller constructor instantiates the database
        /// </summary>
        public ForumController()
        {
            _db = new TechproContext();
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Returns all subforums and retrives the thread count and reply count for each subforum,
        /// along with a link to the last post made inside the subforum.
        /// </summary>
        /// <returns>Forum Index view</returns>
        public ActionResult Index()
        {
            if (SessionHandler.Logon)
                loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            //check if the persistent cookie exists
            HttpCookie _userInfoCookies = Request.Cookies["UserInfo"];
            if (_userInfoCookies != null)
            {
                //set session variables
                SessionHandler.Logon = true;
                SessionHandler.Role = _userInfoCookies["role"];
                SessionHandler.UID = Convert.ToInt32(_userInfoCookies["uid"]);
            }

            //checks if user is admin, if admin, adds the admin forums
            if (SessionHandler.Role == "admin")
                ViewData.Model = _db.Subforums.ToList();
            else
                ViewData.Model = _db.Subforums.Where(i => i.user_access == "user").ToList();

            int subforumCount = _db.Subforums.Count() + 1;

            int[] threadCount = new int[subforumCount];
            int[] postCount = new int[subforumCount];
            int userID;
            Hashtable lastPost = new Hashtable();
            Hashtable users = new Hashtable();

            //collects the data for all the subforums (threads, replies, last post)
            for (int i = 1; i < subforumCount; i++)
            {
                threadCount[i] = _db.ForumThreads.Where(thr => thr.subforum_id == i).Count();
                var forumThreads = from fp in _db.ForumPosts
                                   join ft in _db.ForumThreads
                                   on fp.thread_id equals ft.id
                                   join sf in _db.Subforums
                                   on ft.subforum_id equals sf.id
                                   where sf.id == i
                                   select fp;

                var lastpost = from fp in _db.ForumPosts
                               join ft in _db.ForumThreads
                               on fp.thread_id equals ft.id
                               join sf in _db.Subforums
                               on ft.subforum_id equals i
                               orderby fp.datetime_posted descending
                               select fp;

                lastPost.Add(i, lastpost.FirstOrDefault());

                if (lastpost.FirstOrDefault() != null)
                {
                    userID = lastpost.FirstOrDefault().author_id;
                    if (!users.ContainsKey(userID))
                        users.Add(userID, _db.Users.SingleOrDefault(j => j.id == userID).username);
                }

                postCount[i] = forumThreads.Count();
            }

            //sets all the view parameters
            ViewBag.ThreadCount = threadCount;
            ViewBag.PostCount = postCount;
            ViewBag.LastPost = lastPost;
            ViewBag.Users = users;

            return View();
        }
    }
}

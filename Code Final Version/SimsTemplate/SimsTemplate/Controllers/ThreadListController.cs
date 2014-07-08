using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using System.Collections;
using SimsTemplate;

namespace Forum.Controllers
{
    /// <summary>
    /// Thread List Controller
    /// @Author: Patrick Tseng
    /// </summary>
    public class ThreadListController : Controller
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
        /// @Author: Patrick Tseng
        /// Grabs all the threads given a subforum ID
        /// </summary>
        /// <param name="id">subforum ID</param>
        /// <returns>View</returns>
        public ActionResult Thread(int id)
        {
            if(SessionHandler.Logon)
                loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            Subforum sf = _db.Subforums.SingleOrDefault(i => i.id == id);
            if (sf == null)
            {
                return View("Error", new ArgumentException("Subforum does not exist!"));
            }

            if (sf.user_access == "admin" && SessionHandler.Role != "admin")
            {
                return View("Error", new ArgumentException("You do not have sufficient credentials to view this page."));
            }

            ViewBag.SubforumName = _db.Subforums.Single(i => i.id == id).title;     //gets the subforum name according to the id
            ViewBag.SubforumID = id;
            ViewBag.UserID = SessionHandler.UID;
            var threads    = from d in _db.ForumThreads
                             where d.subforum_id == id
                             select d;
            int userID;
            
            //required because the author ID isn't a foreign key to the User table, D'oh!
            Hashtable postAuthors = new Hashtable();
            Hashtable lastPost = new Hashtable();
            Hashtable replies = new Hashtable();
            Hashtable users = new Hashtable();

            //gets all required information for threads
            foreach (var thread in threads)
            {
                postAuthors.Add(thread.id, _db.Users.Single(i => i.id == thread.author_id).username);
                var posts = from p in _db.ForumPosts
                            where p.thread_id == thread.id
                            orderby p.datetime_posted descending
                            select p;
                lastPost.Add(thread.id, posts.FirstOrDefault());

                if (posts.FirstOrDefault() != null)
                {
                    userID = posts.FirstOrDefault().author_id;
                    if (!users.ContainsKey(userID))
                        users.Add(userID, _db.Users.SingleOrDefault(j => j.id == userID).username);
                }

                replies.Add(thread.id, posts.Count() - 1);
            }

            ViewBag.PostAuthors = postAuthors;
            ViewBag.LastPost = lastPost;
            ViewBag.Replies = replies;
            ViewBag.Users = users;

            return View(threads.ToList());
        }

        /// <summary>
        /// @Author: Patrick Tseng
        /// Deletes a thread if the user is the post author or admin
        /// </summary>
        /// <param name="id">thread to delete</param>
        /// <returns>Redirect to subforum</returns>
        public ActionResult Delete(int id)
        {
            if (!SessionHandler.Logon)
                return RedirectToAction("Index", "Login");

            int subforum_id = 0;
            ForumThread ft = _db.ForumThreads.SingleOrDefault(i => i.id == id);

            if (ft != null)
            {
                var forumPosts = from fp in _db.ForumPosts
                                 where fp.thread_id == id
                                 select fp;

                foreach (ForumPost fp in forumPosts)
                    _db.ForumPosts.Remove(fp);

                subforum_id = ft.subforum_id;

                _db.ForumThreads.Remove(ft);

                _db.SaveChanges();
            }

            return RedirectToAction("Thread", new { id = subforum_id });
        }
    }
}

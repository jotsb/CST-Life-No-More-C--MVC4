using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// @Author: Patrick Tseng
    /// </summary>
    public class NewsFeedController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        ///  @Author: Patrick Tseng
        /// Gets the latest news post for the home page and returns a partial view
        /// containing it.
        /// </summary>
        /// <returns>Partial view of news</returns>
        public ActionResult Index()
        {
            //getting the ID of the subforum that contains the news feeds
            Subforum newsFeedSubforum = db.Subforums.SingleOrDefault(i => i.title == "News Feed");

            if (newsFeedSubforum == null)
            {
                ViewBag.NewsAvailable = false;
                return PartialView();
            }

            int newsfeedID = db.Subforums.SingleOrDefault(i => i.title == "News Feed").id;

            ForumThread latestNewsThread = db.ForumThreads.OrderByDescending(i => i.datetime_posted).FirstOrDefault(i => i.subforum_id == newsfeedID);

            if (latestNewsThread == null)
            {
                ViewBag.NewsAvailable = false;
                return PartialView();
            }

            ViewBag.ForumThread = latestNewsThread;
            ViewBag.ForumPost = db.ForumPosts.Where(i => i.thread_id == latestNewsThread.id).OrderBy(i => i.datetime_posted).First();
            ViewBag.NewsAvailable = true;

            return PartialView();
        }

    }
}
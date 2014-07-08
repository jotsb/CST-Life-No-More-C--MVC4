using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// @Author: Patrick Tseng
    /// Message Count Controller
    /// </summary>
    public class MessageCountController : Controller
    {
        /// <summary>
        /// Database
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Patrick Tseng
        /// Returns the number of unread messages for the user
        /// </summary>
        /// <returns>Parital View</returns>
        public ActionResult Index()
        {
            int unreadCount =     db.UserMessages.Where(i => i.recipient_id == SessionHandler.UID)
                                                 .Where(i => i.message_read == false)
                                                 .Count();

            ViewBag.UnreadCount = unreadCount.ToString();

            ViewBag.DisplayUnread = (unreadCount == 0) ? false : true;

            return PartialView();
        }

    }
}

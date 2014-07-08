using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Models.ViewModels;


namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Taylor
    /// </summary>
    public class ChatController : Controller
    {
        //
        // GET: /Chat/
        HeartbeatMonitor loginTimer;

        /// <summary>
        /// @Author: Taylor
        /// Database connection.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Taylor
        /// Launches the chat client. Sets session data for the client to be retrieved by the ChatValidatorController during authentication
        /// </summary>
        /// <returns>The Chat Client</returns>
        public ActionResult Index()
        {
            List<String> friendsList = new List<String>();

            var uid = SessionHandler.UID;

            //If the uid is negative one, the user must not be logged in.
            if (uid == -1)
            {
                return RedirectToAction("Log_in", "Login");
            }

            User user = (from u in db.Users
                         where u.id == uid
                         select u).FirstOrDefault();

            try
            {
                List<int> friends = (from friendslist in db.FriendsLists
                                     where friendslist.id == SessionHandler.UID
                                     select friendslist.friend_id
                               ).ToList();



                foreach (var friend in friends)
                {
                    User u = (from fUser in db.Users
                              where friend.Equals(fUser.id)
                              orderby fUser.username
                              select fUser).FirstOrDefault();
                    friendsList.Add(u.username);
                    //list.Add(friend.User);
                }
            }
            catch (Exception ex)
            {

            }
            SessionHandler.Username = user.username;
            SessionHandler.Banned = user.is_banned;
            SessionHandler.FriendsList = friendsList;

            loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            bool isAdmin = false;

            if (user.role == "admin")
            {
                isAdmin = true;
            }
            
            SessionHandler.IsAdmin = isAdmin;

            return View("Chat");
        }

        /// <summary>
        /// @Author: Taylor
        /// Redirects to ChatValidator
        /// </summary>
        /// <returns>The result of ChatValidator</returns>
        public ActionResult Validate()
        {
            return RedirectToAction("Index","ChatValidator");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// @Author: Taylor
    /// </summary>
    public class ChatValidatorController : Controller
    {
        
        /// <summary>
        /// Database connection.
        /// </summary>
        TechproContext db = new TechproContext();

        private const String CHAT_SYSTEM_PATH = @"C:\Users\Universal\Documents\BCIT\Term 4\Tech Pro 2\Project\SVn\trunk\web\dev\Chat System\server.py";

        /// <summary>
        /// @Author: Taylor
        /// Default actions for controller. Called from the websocket to get all relevant user data.
        /// </summary>
        /// <returns>A blank page with a response of user data, wrapped in a json object.</returns>
        public ActionResult Index()
        {
            Response.ContentType = "text/plain";

            String hostname = Request.UserHostName;
            AuthenticationResponseData data;
            String username = SessionHandler.Username;
            //if there is no username, cannot be authenticated.
            if (username == null)
            {
                data = new AuthenticationResponseData() { authenticated = false };
            }
            else
            {
                //Create a ResponseData object will all fields required by chat server
                data = new AuthenticationResponseData()
                {
                    username = username,
                    banned = SessionHandler.Banned,
                    authenticated = true,
                    admin = SessionHandler.IsAdmin,
                    friendsList = SessionHandler.FriendsList
                };
            }

            JavaScriptSerializer json = new JavaScriptSerializer();

            Response.Write(json.Serialize(data));
            
            return View("ChatSessionValidator");
        }

        /// <summary>
        /// @Author: Taylor
        /// Occurs when admin wants to ban a user. Parameter is retreived from the url querystring. 
        /// </summary>
        /// <returns>A json object containing whether the user was successfully banned</returns>
        public ActionResult BanUser()
        {
            string username = Request.QueryString["Username"];

            bool isBanned = false;

            if (SessionHandler.IsAdmin)
            {

                //If the username is null, the user cannot be banned.
                if (username == null)
                {
                    isBanned = false;
                }
                else
                {
                    User user = (from u in db.Users
                                 where u.username.Equals(username)
                                 select u).FirstOrDefault();
                    //If the user was found
                    if (user != null)
                    {
                        user.is_banned = true;
                        //Try to save changes to user
                        try
                        {
                            db.SaveChanges();
                            isBanned = true;
                        }
                        catch (Exception ex)
                        {
                            isBanned = false;
                        }
                    }
                }
                BanUserResponseData data = new BanUserResponseData() { banned = isBanned };

                JavaScriptSerializer json = new JavaScriptSerializer();

                Response.Write(json.Serialize(data));

            }

            return View("ChatSessionValidator");
        }

        /// <summary>
        /// @Author: Taylor
        /// Occurs when admin wants to unban a user. Parameter is retreived from the url querystring. 
        /// </summary>
        /// <returns>A json object containing whether the user was successfully unbanned</returns>
        public ActionResult UnbanUser()
        {
            string username = Request.QueryString["Username"];

            bool isBanned = true;
            if (SessionHandler.IsAdmin)
            {
                //If the username is null, the user cannot be unbanned.
                if (username == null)
                {
                    isBanned = true;
                }
                else
                {
                    User user = (from u in db.Users
                                 where u.username.Equals(username)
                                 select u).FirstOrDefault();

                    user.is_banned = false;
                    //Try to save changes to user
                    try
                    {
                        db.SaveChanges();
                        isBanned = false;
                    }
                    catch (Exception ex)
                    {
                        isBanned = true;
                    }
                }

                BanUserResponseData data = new BanUserResponseData() { banned = isBanned };

                JavaScriptSerializer json = new JavaScriptSerializer();

                Response.Write(json.Serialize(data));
            }
               
            return View("ChatSessionValidator");
        }

        
        
        /// <summary>
        /// @Author: Taylor
        /// A class that can be serialized that wraps all relevant user data required by the chat server.
        /// </summary>
        private class AuthenticationResponseData
        {
            public String username;

            public bool authenticated;

            public bool banned;

            public bool admin;

            public List<String> friendsList;
            
        }

        /// <summary>
        /// @Author: Taylor
        /// A class that can be serialized that represents if the user was successfully banned.
        /// </summary>
        private class BanUserResponseData
        {
            public bool banned;
        }


    }
}

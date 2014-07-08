using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Helper;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Jivanjot Brar
    /// @Date  : Oct 30, 2012
    /// 
    /// Controller for the Friends List Feature. Contains various function for the various features of the friends list.
    /// </summary>
    public class FriendListController : Controller
    {
        //database connection
        TechproContext db = new TechproContext();
        // generic list for storing User values from the database
        List<User> list = new List<User>();
        // generic list for storing list of online users from the database.
        List<User> onlineUserList = new List<User>();


        /// <summary>
        /// @Author: Jivanjot Brar
        /// GET: returns the list of friends associated with the user.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
            Dictionary<string, List<User>> dict = new Dictionary<string, List<User>>();
            if (SessionHandler.Logon)
            {
                try
                {
                    List<int> usersOnline = HeartbeatMonitor.getOnlineUsers(HttpContext);

                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
                                         select friendslist.friend_id
                                   ).ToList();

                    foreach (var friend in friends)
                    {
                        User u = (from user in db.Users
                                  where friend.Equals(user.id)
                                  select user).FirstOrDefault();
                        list.Add(u);
                    }

                    foreach (var userO in usersOnline)
                    {
                        User u = (from user in db.Users
                                  where userO == user.id
                                  select user).FirstOrDefault();
                        onlineUserList.Add(u);
                    }

                    dict.Add("friends", list);
                    dict.Add("online", onlineUserList);

                    ViewBag.Message = "Current Friends";
                    return View("Index", dict);
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                //must go to the login action
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// Get: Gives ability to remove friends from the list.
        /// Gets the list of users who are friends with the logged in user and passes it
        /// to the view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            if (SessionHandler.Logon)
            {
                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
                                         select friendslist.friend_id
                                   ).ToList();

                    foreach (var friend in friends)
                    {
                        User u = (from user in db.Users
                                  where friend.Equals(user.id)
                                  select user).FirstOrDefault();
                        list.Add(u);
                    }

                    ViewBag.Message = "Click \"Remove\" to remove any friends";
                    return View("EditFriendList", list);
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                //must go to the login action
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// POST: based on the id passed from the Edit function above this function
        /// takes and removes the row from the database.
        /// </summary>
        /// <param name="fm">Information recieved from the form</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(FormCollection fm)
        {
            if (SessionHandler.Logon)
            {
                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    int friendID = Convert.ToInt32(fm.GetKey(0));

                    var fr = (from f in db.FriendsLists
                              where f.id == SessionHandler.UID && f.friend_id == friendID
                              select f).FirstOrDefault();
                    if (fr != null)
                    {
                        db.FriendsLists.Remove(fr);
                    }

                    fr = (from f in db.FriendsLists
                          where f.id == friendID && SessionHandler.UID == f.friend_id
                          select f).FirstOrDefault();

                    if (fr != null)
                    {
                        db.FriendsLists.Remove(fr);
                    }

                    db.SaveChanges();


                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
                                         select friendslist.friend_id
                                   ).ToList();
                    ViewBag.Message = "Click \"Remove\" to remove any friends";

                    if (friends != null)
                    {

                        foreach (var friend in friends)
                        {
                            User u = (from user in db.Users
                                      where friend.Equals(user.id)
                                      select user).FirstOrDefault();
                            list.Add(u);
                        }

                        return View("EditFriendList", list);
                    }
                    else
                    {
                        return View("EditFriendList");
                    }


                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                //must go to the login action
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// GET: Gets the list of users, except those who are your friends and also yourself(the logged-in user)
        /// and passes the dictionary of list of users to the view. 
        /// 
        /// It also passes the list of users to whom the request is sent and hasn't yet been accepted or declined.
        /// 
        /// Dictionary<"string", List<User>> dict
        /// dict.Add("nonFriends", list of no friends);
        /// dict.Add("pendingRequests", list of pending request);
        /// </summary>
        /// <returns></returns>
        public ActionResult Find()
        {
            List<User> findList = new List<User>();
            List<User> pendingList = new List<User>();
            if (SessionHandler.Logon)
            {
                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    Dictionary<string, List<User>> dictFindFriends = new Dictionary<string, List<User>>();
                    List<FriendsList> friends = (from friendslist in db.FriendsLists
                                                 where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
                                                 select friendslist
                                           ).ToList();

                    List<FriendsList> pending = (from friendslist in db.FriendsLists
                                                 where friendslist.id == SessionHandler.UID && friendslist.status.Equals("pending")
                                                 select friendslist
                                           ).ToList();

                    foreach (var friend in friends)
                    {
                        User u = (from user in db.Users
                                  where friend.friend_id == user.id
                                  select user).FirstOrDefault();
                        findList.Add(u);
                    }

                    foreach (var pend in pending)
                    {
                        User u = (from user in db.Users
                                  where pend.friend_id == user.id
                                  select user).FirstOrDefault();
                        pendingList.Add(u);
                    }

                    // Display a list of users not including the active user
                    List<User>  userList = (
                        from user in db.Users
                        where user.id != SessionHandler.UID
                        orderby user.username ascending
                        select user
                        ).ToList();

                    var list2 = userList.Except(findList);
                    ViewBag.Message = "Find Friends";

                    dictFindFriends.Add("nonFriends", list2.ToList());
                    dictFindFriends.Add("pending", pendingList);


                    return View("FindFriends", dictFindFriends);
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                //must go to the login action
                return RedirectToAction("Index", "Login");
            }
        }


        /// <summary>
        /// @Author: Jivanjot Brar
        /// POST: If the user clicks add friend, this function is called passing in the user ID
        /// of the user to whom the request is going to be sent.
        /// 
        /// This function add a request in the friendslist table in the database and also sends
        /// a message to the user to accept and decline the request.
        /// </summary>
        /// <param name="fm">Gets the data collection from the form.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Find(FormCollection fm)
        {
            bool sendRequest = true;
            if (SessionHandler.Logon)
            {

                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    //display a list of users not including the active user
                    int temp = Convert.ToInt32(fm.GetKey(0));

                    FriendsList friend = (from f in db.FriendsLists
                                          where f.id == SessionHandler.UID && f.friend_id == temp
                                          select f).FirstOrDefault();

                    if (friend != null)
                    {
                        if (friend.num_requests_sent < 10)
                        {
                            int i = friend.num_requests_sent;
                            friend.num_requests_sent = ++i;
                            friend.status = "pending";
                        }
                        else
                        {
                            sendRequest = false;
                        }
                    }
                    else
                    {
                        // Add a request to the friends list table
                        FriendsList fl = new FriendsList();
                        fl.id = SessionHandler.UID;
                        fl.friend_id = temp;
                        fl.num_requests_sent = 1;
                        fl.status = "pending";

                        db.FriendsLists.Add(fl);
                    }

                    if (sendRequest)
                    {
                        // get current users information
                        User c_user = (from userT in db.Users
                                       where userT.id == SessionHandler.UID
                                       select userT).FirstOrDefault();

                        // current users image
                        string userImage = c_user.image_url;

                        //send friend request message to the select user in find friends form.
                        UserMessage um = new UserMessage();

                        // current date and time, when message was sent
                        um.datetime_sent = DateTime.Now;

                        //this is the ID of the current logged in user
                        um.sender_id = SessionHandler.UID;

                        //This is the ID of the user, who is being sent the friend request.
                        um.recipient_id = temp;
                        um.subject = "Friend Request";
                        um.message = "<table><tr>" +
                            "<tdborder=\"0px\"> <img src=\"" + "../" + userImage + "\" height=\"50px\" width=\"45px\"/> </td>" +
                            "<td>" + c_user.firstname + " " + c_user.lastname + " has sent you a friend request. </td>" +
                            "<td><a href=\"../../../FriendList/AcceptRequest/?friendID=" + SessionHandler.UID + "&userID=" + temp + "\">Accept</a></td>" +
                            "<td><a href=\"../../../FriendList/DeclineRequest/?friendID=" + SessionHandler.UID + "&userID=" + temp + "\">Decline</a></td>" +
                            "</tr></table>";
                        um.message_read = false;

                        // add the message to the user messages table
                        db.UserMessages.Add(um);

                        // save the changes made to the database.
                        db.SaveChanges();
                    }

                    // Redirect to specified view
                    return RedirectToAction("Find", "FriendList");

                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                //must go to the login action
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// POST: Allows the user to remove the sent request, if the person hasn't responded to the request.
        /// </summary>
        /// <param name="userID">ID of the user to whom the friend request is sent.</param>
        /// <returns></returns>
        public ActionResult RemoveRequest(int friendID)
        {
            try
            {
                HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                FriendsList friend = (from f in db.FriendsLists
                                      where f.id == SessionHandler.UID && f.friend_id == friendID
                                      select f).FirstOrDefault();

                if (friend != null)
                {
                    db.FriendsLists.Remove(friend);

                    db.SaveChanges();
                }

                return RedirectToAction("Find", "FriendList");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// POST: In the message sent to the user, if the user chooses decline friends request,
        /// this function access the row in the friendslist table in the database and changes the 
        /// status to "denied".
        /// </summary>
        /// <param name="friendID">Takes in the the id of the user who sent the request.</param>
        /// <param name="userID">Id of the user to whom the request was sent.</param>
        /// <returns></returns>
        public ActionResult DeclineRequest(int friendID, int userID)
        {
            try
            {
                HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                var friend = (from fr in db.FriendsLists
                              where fr.id == friendID && fr.friend_id == userID
                              select fr).FirstOrDefault();
                if (friend != null)
                {
                    friend.status = "denied";
                    db.SaveChanges();
                }

                // Redirect to specified view
                return RedirectToAction("Index", "FriendList");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// POST: if the user chooses Accept friend request, this function takes the id of the user
        /// who chooses "accept" and who sent the request and adds the row to the friendslist table
        /// and also access the row entered by the user who sent the request and changes the status
        /// from "pending" to "accepted"
        /// </summary>
        /// <param name="friendID">Takes in the the id of the user who sent the request.</param>
        /// <param name="userID">Id of the user to whom the request was sent.</param>
        /// <returns></returns>
        public ActionResult AcceptRequest(int friendID, int userID)
        {
            try
            {
                HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                var friend = (from fr in db.FriendsLists
                              where fr.id == userID && fr.friend_id == friendID
                              select fr).FirstOrDefault();

                if (friend != null)
                {
                    friend.status = "accepted";
                }
                else
                {
                    FriendsList fl = new FriendsList();
                    fl.id = userID;
                    fl.friend_id = friendID;
                    fl.num_requests_sent = 0;
                    fl.status = "accepted";
                    db.FriendsLists.Add(fl);
                }

                friend = (from fr in db.FriendsLists
                          where fr.id == friendID && fr.friend_id == userID
                          select fr).FirstOrDefault();

                if (friend != null)
                {
                    friend.status = "accepted";
                }

                var user = (from usr in db.Users
                            where usr.id == userID
                            select usr).FirstOrDefault();

                string userImage = user.image_url;

                UserMessage um = new UserMessage();
                um.datetime_sent = DateTime.Now;
                um.sender_id = userID;
                um.recipient_id = friendID;
                um.subject = "Friend Request Accepted";
                um.message = "<table><tr>" +
                    "<td border=\"0px\"> <img src=\"" + "../" + userImage + "\" height=\"50px\" width=\"45px\" /> </td>" +
                    "<td>" + user.firstname + " " + user.lastname + " has accepted your friend request. </td>" +
                    "</tr></table>";
                um.message_read = false;

                // add the message to the user messages table
                db.UserMessages.Add(um);

                // save the changes made to the database
                db.SaveChanges();

                // Redirect to specified view
                return RedirectToAction("Index", "FriendList");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @Author: Jivanjot Brar
        /// Takes in the input and searches the database for either the user's firstname or lastname,
        /// or searches based on the user's username and returns the result to the view.
        /// </summary>
        /// <param name="friendSearchTxt">Value from the search text box</param>
        /// <param name="searchMethod">Value from the dropdown, which contains where to search based on the username or firstname lastname.</param>
        /// <returns></returns>
        public ActionResult Search(string friendSearchTxt, string searchMethod)
        {
            HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
            ViewBag.Message = "Friend Search Results";
            List<User> result = new List<User>();
            List<User> pendingList = new List<User>();
            List<User> friendsList = new List<User>();
            List<User> me = new List<User>();
            Dictionary<string, List<User>> searchResult = new Dictionary<string, List<User>>();
            if (SessionHandler.Logon)
            {
                string searchTxt = SecurityHelper.StripHTML(friendSearchTxt.ToLower());
                try
                {
                    List<FriendsList> pending = (from friendslist in db.FriendsLists
                                                 where friendslist.id == SessionHandler.UID && friendslist.status.Equals("pending")
                                                 select friendslist
                                           ).ToList();

                    List<FriendsList> friends = (from friendslist in db.FriendsLists
                                                 where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
                                                 select friendslist
                                           ).ToList();

                    if (pending != null)
                    {
                        foreach (var pend in pending)
                        {
                            User u = (from user in db.Users
                                      where pend.friend_id == user.id
                                      select user).FirstOrDefault();
                            pendingList.Add(u);
                        }
                    }

                    if (friends != null)
                    {
                        foreach (var friend in friends)
                        {
                            User u = (from user in db.Users
                                      where friend.friend_id == user.id
                                      select user).FirstOrDefault();
                            friendsList.Add(u);
                        }
                    }

                    if (searchMethod.Equals("name"))
                    {
                        // search based on first and last name
                        result = (from user in db.Users
                                  where user.firstname.ToLower().Equals(searchTxt) ||
                                  user.lastname.ToLower().Equals(searchTxt)
                                  select user).ToList();

                    }
                    else if (searchMethod.Equals("uname"))
                    {
                        // search based on the username
                        result = (from user in db.Users
                                  where user.username.ToLower().Equals(searchTxt)
                                  select user).ToList();
                    }

                    if (result.Count > 0)
                    {
                        me = (from user in db.Users
                              where user.id == SessionHandler.UID
                              select user).ToList();

                        var result2 = result.Except(me);
                        searchResult.Add("searchResult", result2.ToList());
                        searchResult.Add("pending", pendingList);
                        searchResult.Add("friend", friendsList);
                        return View("SearchFriends", searchResult);
                    }
                    else
                    {
                        searchResult.Add("searchResult", result);
                        searchResult.Add("pending", pendingList);
                        searchResult.Add("friend", friendsList);

                        ViewBag.SearchMessage = "No results found for your query: \" " + friendSearchTxt + " \"";
                        return View("SearchFriends", searchResult);
                    }
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}

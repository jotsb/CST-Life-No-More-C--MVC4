using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// Profile Controller, retrieves character attributes and friends list data.
    /// 
    /// @author Shan Bains
    /// </summary>
    public class ViewProfileController : Controller
    {

        //database connection
        TechproContext db = new TechproContext();
        int profileId = -1;
        //
        // GET: /Profile/
        /// <summary>
        /// @author Shan Bains
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(String id)
        {
            //profileId = 64;
            profileId = Convert.ToInt32(id);
            // if logged on
            if ((SessionHandler.Logon) && (profileId > -1))
            {
                try
                {
                    // list of characters
                    List<Character> list1 = new List<Character>();
                    // dictionary with user and character data
                    Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>> dictionary =
                         new Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>>();

                    Dictionary<String, List<User>> userDict = new Dictionary<String, List<User>>();
                    Dictionary<String, List<Character>> charDict = new Dictionary<String, List<Character>>();

                    //query to get the current user
                    User currentUser = (from u in db.Users
                                        where u.id == profileId
                                        select u).FirstOrDefault();

                    // get the list of the users friends as <Users>
                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == profileId && friendslist.status.Equals("accepted")
                                         select friendslist.friend_id).ToList();

                    // holds the list of friends
                    List<User> CurrentUserFriends = new List<User>();

                    if (friends.Count <= 0)
                    {
                        User noFriend = new User();
                        noFriend.username = "N/A";
                        noFriend.firstname = "No";
                        noFriend.lastname = "Friends";
                        CurrentUserFriends.Add(noFriend);
                    }
                    else
                    {

                        foreach (var friend in friends)
                        {
                            User u = (from user in db.Users
                                      where friend.Equals(user.id)
                                      orderby user.username
                                      select user).FirstOrDefault();
                            CurrentUserFriends.Add(u);
                        }
                    }

                    // add current user and user's friends to dictionary
                    List<User> currentUserList = new List<User>();
                    currentUserList.Add(currentUser);
                    userDict.Add("currentUser", currentUserList);
                    userDict.Add("friends", CurrentUserFriends);


                    // query to get the current character from db
                    Character c_character = (from Character in db.Characters
                                             where Character.id == currentUser.current_character
                                             select Character).FirstOrDefault();

                    List<Character> currentCharacters = new List<Character>();
                    currentCharacters.Add(c_character);
                    // query to get all the users characters from the db
                    List<Character> userCharacters = (from Character in db.Characters
                                                      where Character.user_id == profileId
                                                      select Character).ToList();

                    charDict.Add("mainCharacter", currentCharacters);
                    charDict.Add("otherCharacters", userCharacters);

                    dictionary.Add(userDict, charDict);

                    return View("~/Views/Profile/ViewProfile.cshtml", dictionary);
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

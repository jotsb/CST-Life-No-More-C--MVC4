using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Helper;

namespace SimsTemplate.Controllers
{
    /// <summary>
    /// @author Shan Bains
    /// Profile Controller, retrieves character attributes and friends list data.
    /// gets all character information, and friends list information. 
    /// 
    /// </summary>
    public class ProfileController : Controller
    {

        //database connection
        TechproContext db = new TechproContext();
        //
        // GET: /Profile/

        /// <summary>
        /// @author Shan Bains
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // if logged on
            if (SessionHandler.Logon)
            {
                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    // list of characters
                    List<Character> list1 = new List<Character>();
                    // dictionary with user and character data
                    Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>> dictionary =
                         new Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>>();

                    Dictionary<String, List<User>> userDict = new Dictionary<String, List<User>>();
                    Dictionary<String, List<Character>> charDict = new Dictionary<String, List<Character>>();

                    //query to get the current user
                    User currentUser = (from u in db.Users
                                        where u.id == SessionHandler.UID
                                        select u).FirstOrDefault();

                    // get the list of the users friends as <Users>
                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
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
                                                      where Character.user_id == SessionHandler.UID
                                                      select Character).ToList();

                    charDict.Add("mainCharacter", currentCharacters);
                    charDict.Add("otherCharacters", userCharacters);

                    dictionary.Add(userDict, charDict);

                    return View("Index", dictionary);
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

        /// <summary>
        /// @author Shan Bains
        /// edit profile page - updates the user profile information
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            // if logged on
            if (SessionHandler.Logon)
            {

                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    // list of characters
                    List<Character> list1 = new List<Character>();
                    // dictionary with user and character data
                    Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>> dictionary =
                         new Dictionary<Dictionary<String, List<User>>, Dictionary<String, List<Character>>>();

                    Dictionary<String, List<User>> userDict = new Dictionary<String, List<User>>();
                    Dictionary<String, List<Character>> charDict = new Dictionary<String, List<Character>>();

                    //query to get the current user
                    User currentUser = (from u in db.Users
                                        where u.id == SessionHandler.UID
                                        select u).FirstOrDefault();

                    // get the list of the users friends as <Users>
                    List<int> friends = (from friendslist in db.FriendsLists
                                         where friendslist.id == SessionHandler.UID && friendslist.status.Equals("accepted")
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
                                                      where Character.user_id == SessionHandler.UID
                                                      select Character).ToList();

                    charDict.Add("mainCharacter", currentCharacters);
                    charDict.Add("otherCharacters", userCharacters);

                    dictionary.Add(userDict, charDict);

                    return View("EditProfile", dictionary);
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

        /// <summary>
        /// @author Shan Bains
        /// name validation function checks for all letters and if the textbox is null or empty. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Boolean nameValidation(String name)
        {
            name.Trim();
            Boolean isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                return isValid;
            }

            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetter(name[i]))
                {
                    isValid = false;
                    return isValid;
                }
            }

            return isValid;
        }
        
        /// <summary>
        /// @author Shan Bains
        /// age validation field that ensures all data is numeric and within range
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        public Boolean ageValidation(String age)
        {
            age.Trim();
            Boolean isValid = true;

            if (string.IsNullOrEmpty(age))
            {
                isValid = false;
                return isValid;
            }

            for (int i = 0; i < age.Length; i++)
            {
                if (!char.IsDigit(age[i]))
                {
                    isValid = false;
                    return isValid;
                }
            }

            int ageval = Convert.ToInt32(age);

            if ((ageval <= 0) || (ageval > 100))
            {
                isValid = false;
                return isValid;
            }

            return isValid;
        }


        /// <summary>
        /// @author Shan Bains
        /// validation function that ensures the correct sex value has been entered. 
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public Boolean sexValidation(String sex)
        {
            sex.Trim();
            Boolean isValid = true;

            if (string.IsNullOrEmpty(sex))
            {
                isValid = false;
                return isValid;
            }

            for (int i = 0; i < sex.Length; i++)
            {

                if ((sex[i] != 'm') && (sex[i] != 'M')
                     && (sex[i] != 'f') && (sex[i] != 'F'))
                {

                    isValid = false;
                    return isValid;
                }
            }

            return isValid;
        }

        /// <summary>
        /// HttpPost - function that handles all the http requests for the edit profile page. 
        /// gets data, validates it, and updates the user information table in the database, if valid. 
        /// if the data is invalid it will refresh the edit profile page.
        /// 
        /// @author Shan Bains
        /// </summary>
        /// <param name="fm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(FormCollection fm)
        {
            if (SessionHandler.Logon)
            {
                try
                {
                    HeartbeatMonitor loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);
                    //array to hold post data
                    string[] formInfo = new string[5];

                    // get post data from form
                    formInfo[0] = SecurityHelper.StripHTML(fm.GetValues("firstnameTextBox")[0]);
                    formInfo[1] = SecurityHelper.StripHTML(fm.GetValues("lastnameTextBox")[0]);
                    formInfo[2] = SecurityHelper.StripHTML(fm.GetValues("ageTextBox")[0]);
                    formInfo[3] = SecurityHelper.StripHTML(fm.GetValues("sexTextBox")[0]);
                    formInfo[4] = SecurityHelper.StripHTML(fm.GetValues("locationTextBox")[0]);

                    if ((nameValidation(formInfo[0])) && (nameValidation(formInfo[1]))
                        && (ageValidation(formInfo[2])) && (sexValidation(formInfo[3]))
                        && (nameValidation(formInfo[4])))
                    {
                        string[] validInfo = new String[5];

                        Array.Copy(formInfo, validInfo, 5);

                        // get the current user to update in db
                        User currentUser = (from u in db.Users
                                            where u.id == SessionHandler.UID
                                            select u).FirstOrDefault();

                        // update user information
                        currentUser.firstname = validInfo[0];
                        currentUser.lastname = validInfo[1];
                        currentUser.age = Int32.Parse(validInfo[2]);
                        currentUser.sex = validInfo[3].Substring(0, 1);
                        currentUser.location = validInfo[4];

                        // db.Users.Add(currentUser);
                        db.SaveChanges();
                        Session["displayErrors"] = false;
                        // go back to profile page
                        return RedirectToAction("Index", "Profile");

                    }
                    else
                    {
                        //ModelState.AddModelError("firstnameTextBox", "You have entered invalid info, please go back and enter valid information.");
                        ViewBag.error = "You have entered invalid info, please go back and enter valid information.";
                        Session["displayErrors"] = true;
                        // go back to profile page
                        return RedirectToAction("Edit", "Profile");
                        // return View("~/Views/Profile/EditProfile.cshtml");
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

    }
}

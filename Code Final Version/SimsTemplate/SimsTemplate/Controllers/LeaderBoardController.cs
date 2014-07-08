using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using SimsTemplate.Models.ViewModels;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @author: Guillaume Jacques
    /// @date: November 7, 2012
    /// The leaderboard controller contains the different method allowing the user to choose and to view the 
    /// top 10 characters in the game according to the different options the user has chosen. 
    /// </summary>
    public class LeaderBoardController : Controller
    {

        /// <summary>
        /// Collection from the database context building the model TechproContext.
        /// </summary>
        private TechproContext db = new TechproContext();
        /// <summary>
        /// represents the list of the character returned by the sorting option the user selected.
        /// </summary>
        private List<Character> list = new List<Character>();
        /// <summary>
        /// represents the list of user currently connected.
        /// </summary>
        private List<User> currentUserList;
        
        /// <summary>
        /// represents the user logged activity on the page
        /// </summary>
        private HeartbeatMonitor loginTimer;

        /// <summary>
        /// checking pattern to ensure no illegal entries are entered in the textbox
        /// </summary>
        const string HTML_TAG_PATTERN = "<.*?>";

        /// <summary>
        /// @author: Guillaume Jacques
        /// constructor of the leaderboard controller 
        /// </summary>
        public LeaderBoardController()
        {
            ViewBag.Compare = false;
        }




        //
        // GET: /LeaderBoard/
        /// <summary>
        /// @author: Guillaume Jacques
        /// by default the controller is taking the first 10 players ordered by ascending names
        /// </summary>
        /// <returns>the list of the first 10 characters sorted by name</returns>
        public ActionResult Index()
        {

            //check if the persistent cookie exists
            HttpCookie _userInfoCookies = Request.Cookies["UserInfo"];
            if (_userInfoCookies != null)
            {
                //set session variables
                SessionHandler.Logon = true;
                SessionHandler.Role = _userInfoCookies["role"];
                SessionHandler.UID = Convert.ToInt32(_userInfoCookies["uid"]);
            }

            loginTimer = new HeartbeatMonitor(SessionHandler.UID, HttpContext);

            try
            {
                return View("Index", getScoreList());
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// order by top 10 global score
        /// </summary>
        /// <returns>the list of the first 10 characters sorted by score</returns>
        public ActionResult GetTopGlobalScore()
        {
            try
            {
                return View("Index", getScoreList());
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method selecting and ordering the top 10 characters based on the level of sanity
        /// </summary>
        /// <returns>the list of the first 10 characters sorted by sanity level</returns>
        public List<Character> GetTopSanity()
        {
            try
            {
                var characters = (
                                    from c in db.Characters
                                    orderby c.sanity descending
                                    select c
                                    ).Take(10);

                return AddCurrentUser(characters);
            }
            catch (Exception ex)
            {
                return getScoreList();
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method selecting and ordering the top 10 characters based on the amount of money they possess.
        /// </summary>
        /// <returns>the list of the first 10 characters sorted by amount of money</returns>
        public ActionResult GetTopMoney()
        {
            try
            {
                var characters = (
                                    from c in db.Characters
                                    orderby c.money descending
                                    select c
                                    ).Take(10);

                return View("sortedbyMoney", ReturnOrderedList(characters));
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", ex);
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// define whether or not the user connected has a character
        /// </summary>
        /// <param name="list">represents the lis of all charcters</param>
        /// <returns>return true if the user connected has a character registered and 
        /// already listed in the leaderboard according to the sorting parameter selected</returns>
        public Boolean isInLeaderboard(IEnumerable<Character> list)
        {
            var current = (
                            from u in db.Users
                            where u.id == SessionHandler.UID
                            select u
                          );

            // check authenticated
            if (current.Count() != 0)
            {
                currentUserList = current.ToList();
                foreach (Character c in list)
                {
                    foreach (User u in current)
                    {
                        if (c.id == u.id)
                        {
                            // if there is a character associated with the user logged
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the post value from the dropdown menu and passing to the view the list of character corresponding to the 
        /// sorting choice the user made.
        /// </summary>
        /// <returns>the view with sorted list according to the sorting parameter chosen</returns>
        [HttpPost]
        public ActionResult Sort()
        {
            NameValueCollection nvc = Request.Form;
            int methodCalled = Int32.Parse(nvc.Get("sort"));

            switch (methodCalled)
            {
                case 1:
                    return View("Index", getNameList());
                case 2:
                    return View("Index", getMoneyList());
                case 3:
                    return View("Index", GetTopSanity());
                case 4:
                    return View("Index", getScoreList());
                case 5:
                    return View("Index", getGradesList());
                default:
                    return View("Index", getScoreList());
            }

        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the top 10 character sorted according to the name.
        /// </summary>
        /// <returns>the list of top 10 characters sorted</returns>
        public List<Character> getNameList()
        {
            var characters = (
                    from c in db.Characters
                    orderby c.name ascending
                    select c
                 ).Take(10);
            return AddCurrentUser(characters);
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method checking for the current user and add its character if this character is 
        /// not in the leaderboard already.
        /// </summary>
        /// <param name="characters">list of the characters in the selected set according to 
        /// the sorting method sleected</param>
        /// <returns>list of characters including the current user if not already in the initial 
        /// set of characters</returns>
        private List<Character> AddCurrentUser(IQueryable<Character> characters)
        {
            // check if the current user is in the leaderboard if so get the position
            ViewBag.user_Position = getUserIndex(characters);
            ViewBag.user = (getCurrentCharacter()).id;
            List<Character> cl = characters.Take(10).ToList();
            // if not in leaderboard add to the list
            if (!isInLeaderboard(cl))
            {
                cl.Add(getCurrentCharacter());
                if (ViewBag.user_Position == 10)
                {
                    ViewBag.user_Position = 11;
                }
                return cl;
            }
            return cl;
        }


        /// <summary>
        /// @author: Guillaume Jacques
        /// get the position of the character from the logged user in the list of the query selected for the leaderboard
        /// </summary>
        /// <param name="characters">list of character already sorted according </param>
        /// <returns>ipositionin the leaderboad for the current user's character</returns>
        private Int32 getUserIndex(IQueryable<Character> characters)
        {
            int index = 0;
            Character current = getCurrentCharacter();
            if (current == null)
            {
                return -1;
            }

            else if (SessionHandler.Logon)
            {
                foreach (var c in characters)
                {
                    index++;
                    if (c.id == SessionHandler.UID)
                    {
                        break;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the top 10 character sorted according to the amount of money.
        /// </summary>
        /// <returns>the list of top 10 characters sorted</returns>
        private Character getCurrentCharacter()
        {
            try
            {
                var current = (
                                from c in db.Characters
                                where c.id == SessionHandler.UID
                                select c
                              );

                if (current.Count() != 0)
                {
                    return (Character)current.First();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the top 10 character sorted according to the score.
        /// </summary>
        /// <returns>the list of top 10 characters sorted</returns>
        private List<Character> getScoreList()
        {
            try
            {
                var characters = (
                        from c in db.Characters
                        orderby c.global_score descending
                        select c
                        );

                return AddCurrentUser(characters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the top 10 character sorted according to the grades.
        /// </summary>
        /// <returns>the list of top 10 characters sorted</returns>
        private List<Character> getGradesList()
        {
            var characters = (
                                from c in db.Characters
                                orderby c.grades descending
                                select c
                             ).Take(10);

            return AddCurrentUser(characters);
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the top 10 character sorted according to the amount of money.
        /// </summary>
        /// <returns>the list of top 10 characters sorted</returns>
        private List<Character> getMoneyList()
        {
            var characters = (
                                from c in db.Characters
                                orderby c.money descending
                                select c
                             ).Take(10);

            return AddCurrentUser(characters);
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method used to look for either a character name or a user name
        /// </summary>
        /// <returns>return a list corresponding to the search pattern</returns>
        [HttpPost]
        public ActionResult Search()
        {
            NameValueCollection nvc = Request.Form;
            String searchedName = LeaderBoardController.StripHTML(nvc.Get("search").Split(',')[0]);

            String searchedOptions = LeaderBoardController.StripHTML(nvc.Get("searchOptions"));

            return View("Index", GetSearchList(searchedName, searchedOptions));
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method getting the list of Characters that are matching the search word.
        /// </summary>
        /// <param name="searchedName">keyword used to search the database of character names</param>
        /// <param name="searchedOptions">1 is for a search by character name otherwise by user name</param>
        /// <returns></returns>
        private List<Character> GetSearchList(String searchedName, String searchedOptions)
        {
            searchedName = LeaderBoardController.StripHTML(searchedName);
            if (searchedOptions.Equals("1"))
            {
                var searched = (
                                    from c in db.Characters
                                    where c.name.ToLower() == searchedName.ToLower()
                                    select c
                                );
                return ReturnOrderedList(searched);
            }
            else
            {
                var searched = (
                                    from u in db.Users
                                    join c in db.Characters on u.id equals c.user_id
                                    where (u.firstname.ToLower() == searchedName.ToLower() ||
                                           u.lastname.ToLower() == searchedName.ToLower())
                                    select c
                                );
                return ReturnOrderedList(searched);
            }
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// method creating a list of the characters according to the query chosen by the user
        /// </summary>
        /// <param name="searched">query parameter chosen by the user</param>
        /// <returns>list of Character matching the criteria from suer</returns>
        private List<Character> ReturnOrderedList(IQueryable<Character> searched)
        {
            List<Character> templist = new List<Character>();
            foreach (var c in searched)
            {
                templist.Add(c);
            }
            return templist;
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// handles the comparison of characters selected by the users in the leaderboard and 
        /// </summary>
        /// <returns>list of 2 characters selected after setting the viewbag corresponding variable for comparison</returns>
        [HttpPost]
        public ActionResult Compare(FormCollection fc)
        {
            if (fc.Count != 3)
            {
                ViewBag.MessageCompare = "Select 2 characters to compare.";
                ViewBag.Compare = true;
                return View("Index", getScoreList());
            }
            else
            {
                int firstCharacter = Int32.Parse(fc[1]);
                int secondCharacter = Int32.Parse(fc[2]);

                var characters = (
                                        from c in db.Characters
                                        where (c.id == firstCharacter || c.id == secondCharacter)
                                        select c
                                );
                ViewBag.Compare = false;
                List<Character> templist = new List<Character>();
                foreach (var c in characters)
                {
                    templist.Add(c);
                }

                SetComparisonResult(templist);
                return View("compare", templist);
            }

        }
        /// <summary>
        /// @author: Guillaume Jacques
        /// set all the compared values between the 2 characters selected for comparison
        /// </summary>
        private void SetComparisonResult(List<Character> templist)
        {
            ViewBag.comprison_grade = templist[0].grades - templist[1].grades;
            ViewBag.comprison_money = templist[0].money - templist[1].money;
            ViewBag.comprison_score = templist[0].global_score - templist[1].global_score;
            ViewBag.comprison_hunger = templist[0].hunger - templist[1].hunger;
            ViewBag.comprison_sanity = templist[0].sanity - templist[1].sanity;
            ViewBag.comprison_bladder = templist[0].bladder - templist[1].bladder;
            ViewBag.comprison_fun = templist[0].fun - templist[1].fun;
            ViewBag.comprison_energy = templist[0].energy - templist[1].energy;
        }

        /// <summary>
        /// @author: Guillaume Jacques
        /// check for validity of entry string value
        /// </summary>
        /// <param name="inputString">the string to check</param>
        /// <returns>a string with all the non character entries removed</returns>
        static string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }
    }
}

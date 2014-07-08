using SimsTemplate.Helper;
using SimsTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Matt Fisher
    /// Controller used for displaying the administration
    /// pages. These pages are only accessible by an
    /// authorized user (typically an administrator).
    /// 
    /// The administration pages handle database logic
    /// such as creating a new user account, banning users,
    /// editing users, etc.
    /// </summary>
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        /// <summary>
        /// Database connection.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Matt
        /// Displays all users in the database
        /// </summary>
        /// <returns>a view with all the users in the database</returns>
        public ActionResult Index()
        {
            if (ValidUser())
            {
                try
                {
                    List<SimsTemplate.Models.User> userlist = db.Users.ToList();

                    // sort the list of users by username
                    userlist.Sort(delegate(SimsTemplate.Models.User u1, SimsTemplate.Models.User u2) { return u1.username.CompareTo(u2.username); });

                    // display the sorted list in the view
                    return View(userlist);
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Creates a new user into the database.
        /// </summary>
        /// 
        /// <param name="username">the user's username. Must be unique.</param>
        /// 
        /// <param name="firstname">the user's first name.</param>
        /// 
        /// <param name="lastname">the user's last name</param>
        /// 
        /// <param name="email_formatted">the user's email_formatted. Must be unique.</param>
        /// 
        /// <param name="password">the user's password as plain text. This
        /// will be encrypted before it gets sent to the database.</param>
        /// 
        /// <param name="role">the user's role, such as admin or user</param>
        /// 
        /// <param name="age">the user's age</param>
        /// 
        /// <param name="sex">the user's sex, which is either 'm' or 'f'</param>
        /// 
        /// <param name="location">the user's location, such as Canada or the US</param>
        /// 
        /// <returns>A view returning success or failure. Failure
        /// displays the exception that occurred</returns>
        public ActionResult AddUser(string username,
                                    string firstname,
                                    string lastname,
                                    string email,
                                    string password,
                                    string role,
                                    int age,
                                    string sex,
                                    string location)
        {
            if (ValidUser())
            {
                User user = new User();

                user.username = username;
                user.firstname = firstname;
                user.lastname = lastname;
                user.email = email;
                user.password = AuthenticationHelper.ENCRYPT_ME(password);
                user.role = role;
                user.datetime_registered = DateTime.Now;
                user.age = age;
                user.sex = sex;
                user.location = location;
                user.image_url = "~/Content/Images/a.jpg";
                user.is_banned = false;

                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return View("~/Views/Admin/Success.cshtml");
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Adds a character to the database with default values.
        /// NOTE: The client should check to ensure that the username
        /// exists in the database by using a client-side language
        /// </summary>
        /// <param name="name">the character's name</param>
        /// <param name="sex">the character's sex, which is either 'm' or 'f'</param>
        /// <param name="username">the user that this character is linked to</param>
        /// <returns></returns>
        public ActionResult AddCharacter(string name,
                                         string sex,
                                         string username)
        {
            if (ValidUser())
            {
                Character character = new Character();
                User user = (from u in db.Users
                             where u.username.Equals(username)
                             select u).FirstOrDefault();

                character.name = name;
                character.sex = sex;
                // client should check to make sure that the username exists in the database
                character.user_id = (from u in db.Users
                                     where u.username.Equals(username)
                                     select u.id).FirstOrDefault();
                // add other default values
                character.grades = 0;
                character.money = 0;
                character.position = null;
                character.inventory = null;
                character.hunger = null;
                character.sanity = null;
                character.fun = null;
                character.energy = null;
                character.bladder = null;
                character.global_score = 0;

                try
                {
                    db.Characters.Add(character);
                    db.SaveChanges();
                    // set the user's current character to the newly created character
                    user.current_character = (from c in db.Characters
                                              where c.id.Equals(character.id)
                                              select c.id).First();
                    db.SaveChanges();
                    return View("~/Views/Admin/Success.cshtml");
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// bans the specified user by editing the "banned" field
        /// in the database.
        /// </summary>
        /// <param name="username">the user to ban</param>
        /// <returns>A view returning whether the ban succeeded or failed</returns>
        public ActionResult BanUser(string username)
        {
            if (ValidUser())
            {
                User user = (from u in db.Users
                             where u.username.Equals(username)
                             select u).FirstOrDefault();

                user.is_banned = true;

                try
                {
                    db.SaveChanges();
                    return View("~/Views/Admin/Success.cshtml");
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }
		
		/// <summary>
        /// @Author: Matt
        /// unbans the specified user by editing the "banned" field
        /// in the database.
        /// </summary>
        /// <param name="username">the user to unban</param>
        /// <returns>A view returning whether the unban succeeded or failed</returns>
		public ActionResult UnBanUser(string username)
        {
			if(ValidUser())
			{
				User user = (from u in db.Users
							 where u.username.Equals(username)
							 select u).FirstOrDefault();

				user.is_banned = false;

				try
				{
					db.SaveChanges();
					return View("~/Views/Admin/Success.cshtml");
				}
				catch (Exception ex)
				{
					return View("~/Views/Shared/Error.cshtml", ex);
				}
			}
			else
			{
				return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
			}
        }

        /// <summary>
        /// @Author: Matt
        /// Displays a web form to add a user into the db.
        /// </summary>
        /// <returns>the view that contains the form to add a new user</returns>
        public ActionResult DisplayAddUserForm()
        {
            if (ValidUser())
            {
                return View();
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// displays a web form to add a character into the db
        /// </summary>
        /// <returns>the view that contains the form to add a new character</returns>
        public ActionResult DisplayAddCharacterForm()
        {
            if (ValidUser())
            {
                return View();
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Displays all characters in the database.
        /// </summary>
        /// <returns>a view listing all characters in the db</returns>
        public ActionResult DisplayCharacters()
        {
            if (ValidUser())
            {
                try
                {
                    return View(db.Characters.ToList());
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Grabs the user from the database and edits it according to
        /// the new values.
        /// </summary>
        /// <param name="username">the username of the user.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditUser(string username)
        {
            if (ValidUser())
            {
                User user = (from u in db.Users
                             where u.username.Equals(username)
                             select u).FirstOrDefault();

                return View(user);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Edits the user in the database with the updated values.
        /// </summary>
        /// <param name="id">the id of the user</param>
        /// <param name="firstname">the user's new name</param>
        /// <param name="lastname">the user's new last name</param>
        /// <param name="email_formatted">the user's updated email address</param>
        /// <param name="password">the user's new password, sent as plain text</param>
        /// <param name="role">the user's new role</param>
        /// <param name="age">the user's updated age</param>
        /// <param name="sex">the user's sex</param>
        /// <param name="location">the user's new location</param>
        /// <param name="is_banned">user is banned or not</param>
        /// <returns>a view returning success or failure</returns>
        [HttpPost]
        public ActionResult EditUser(
            int id,
            string firstname,
            string lastname,
            string email,
            string role,
            int age,
            string sex,
            string location)
        {
            if (ValidUser())
            {
                User user = (from u in db.Users
                             where u.id.Equals(id)
                             select u).FirstOrDefault();

                user.firstname = firstname;
                user.lastname = lastname;
                user.email = email;
                user.role = role;
                user.age = age;
                user.sex = sex;
                user.location = location;

                try
                {
                    db.SaveChanges();
                    return View("~/Views/Admin/Success.cshtml");
                }
                catch (Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml", ex);
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ArgumentException("User is not an admin."));
            }
        }

        /// <summary>
        /// @Author: Matt
        /// Tests to ensure if the user can access the admin pages.
        /// This should be called every time the user tries to access
        /// an admin page.
        /// </summary>
        /// <returns>true if the user can access the admin pages, false otherwise.</returns>
        private bool ValidUser()
        {
            return ((SessionHandler.Logon) && (SessionHandler.Role == "admin") && (SessionHandler.Banned == false));
        }
    }
}

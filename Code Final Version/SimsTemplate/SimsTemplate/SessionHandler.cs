using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimsTemplate.Models;

namespace SimsTemplate
{
    /// <summary>
    /// @Author: Aubrey Fowler, Taylor Dixon
    /// @Date: Oct 27, 2012
    /// SessionHandler class for each user session.
    /// </summary>
    public static class SessionHandler
    {

        /// <summary>
        /// Key for user's logon status
        /// </summary>
        private static string _logonKey = "logon";

        /// <summary>
        /// Key for user's role
        /// </summary>
        private static string _roleKey = "role";

        /// <summary>
        /// Key for user's id
        /// </summary>
        private static string _uidKey = "uid";

        /// <summary>
        /// Key for user's username
        /// </summary>
        private static string _usernameKey = "username";

        /// <summary>
        /// Key for if user is banned
        /// </summary>
        private static string _bannedKey = "banned";

        /// <summary>
        /// Key for if user is admin
        /// </summary>
        private static string _adminKey = "admin";

        /// <summary>
        /// Key for friendslist of user
        /// </summary>
        private static string _friendsListKey = "flist";

        /// <summary>
        /// @Author: Aubrey
        /// Checks to see if this key exists for the cookie, sets it in the Session
        /// </summary>
        public static bool Logon
        {
            get
            {
                if (HttpContext.Current.Session[_logonKey] == null)
                {
                    return false;
                }
                return (bool)HttpContext.Current.Session[_logonKey];

            }
            set
            {
                HttpContext.Current.Session[_logonKey] = value;
            }
        }

        /// <summary>
        /// @Author: Aubrey
        /// Checks to see if this key exists for the name, sets it in the Session
        /// </summary>
        public static string Role
        {
            get
            {
                if (HttpContext.Current.Session[_roleKey] == null)
                {
                    return string.Empty;
                }
                return HttpContext.Current.Session[_roleKey].ToString();
            }
            set
            {
                HttpContext.Current.Session[_roleKey] = value;
            }
        }

        /// <summary>
        /// @Author: Aubrey
        /// Checks to see if this key exists for the cookie, sets it in the Session
        /// </summary>
        public static int UID
        {
            get
            {
                if (HttpContext.Current.Session[_uidKey] == null)
                {
                    return -1;
                }
                return (int)HttpContext.Current.Session[_uidKey];
            }
            set
            {
                HttpContext.Current.Session[_uidKey] = value;
            }
        }

        /// <summary>
        /// Represents a user's username
        /// @author Taylor Dixon
        /// </summary>
        public static string Username
        {
            get
            {
                string username = null;
                try
                {
                    username = (string) HttpContext.Current.Session[_usernameKey];
                }
                catch (NullReferenceException e)
                {
                    username = null;
                }

                return username;
            }
            set
            {
                try
                {
                   HttpContext.Current.Session[_usernameKey] = value;
                }
                catch (NullReferenceException e)
                {
                    
                }
            }
        }

        /// <summary>
        /// Represents if the user is banned
        /// @Author Taylor Dixon
        /// </summary>
        public static bool Banned
        {
            get
            {
                bool banned = false;
                try
                {
                    banned = (bool)HttpContext.Current.Session[_bannedKey];
                }
                catch (NullReferenceException e)
                {
                    banned = false;
                }

                return banned;
            }
            set
            {
                try
                {
                    HttpContext.Current.Session[_bannedKey] = value;
                }
                catch (NullReferenceException e)
                {

                }
            }
        }
        
        /// <summary>
        /// Represents if the current user is admin or not.
        /// @author Taylor Dixon
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                bool isAdmin = false;
                try
                {
                    isAdmin = (bool)HttpContext.Current.Session[_adminKey];
                }
                catch (NullReferenceException e)
                {
                    isAdmin = false;
                }

                return isAdmin;
            }
            set
            {
                try
                {
                    HttpContext.Current.Session[_adminKey] = value;
                }
                catch (NullReferenceException e)
                {

                }
            }
        }

        /// <summary>
        /// Represents a list uf users for a particular user
        /// @author Taylor Dixon
        /// </summary>
        public static List<String> FriendsList
        {
            get
            {
                try
                {
                    return (List<String>)HttpContext.Current.Session[_friendsListKey];
                }
                catch (NullReferenceException e)
                {
                    return new List<String>();
                }
            }
            set
            {
                HttpContext.Current.Session[_friendsListKey] = value;
            }
        }
    }

}
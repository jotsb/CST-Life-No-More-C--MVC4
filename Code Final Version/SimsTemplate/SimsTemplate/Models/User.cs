using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class User
    {
        public User()
        {
            this.Characters = new List<Character>();
            this.ForumThreads = new List<ForumThread>();
            this.FriendsLists = new List<FriendsList>();
            this.FriendsLists1 = new List<FriendsList>();
            this.UserMessages = new List<UserMessage>();
            this.UserMessages1 = new List<UserMessage>();
        }

        public int id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public System.DateTime datetime_registered { get; set; }
        public Nullable<int> age { get; set; }
        public string sex { get; set; }
        public string location { get; set; }
        public string image_url { get; set; }
        public bool is_banned { get; set; }
        public Nullable<int> current_character { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual Character Character { get; set; }
        public virtual ICollection<ForumThread> ForumThreads { get; set; }
        public virtual ICollection<FriendsList> FriendsLists { get; set; }
        public virtual ICollection<FriendsList> FriendsLists1 { get; set; }
        public virtual ICollection<UserMessage> UserMessages { get; set; }
        public virtual ICollection<UserMessage> UserMessages1 { get; set; }
    }
}

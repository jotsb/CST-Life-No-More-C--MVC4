using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimsTemplate.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public bool IsBanned { get; set; }

        public UserModel(string name, bool banned)
        {
            Name = name;
            IsBanned = banned;
        }
    }

    public class UserContext
    {
        public List<UserModel> UserList;

        public UserContext()
        {
            UserList = new List<UserModel>();
        }
    }
}
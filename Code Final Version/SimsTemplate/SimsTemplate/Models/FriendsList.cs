using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class FriendsList
    {
        public int id { get; set; }
        public int friend_id { get; set; }
        public int num_requests_sent { get; set; }
        public string status { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}

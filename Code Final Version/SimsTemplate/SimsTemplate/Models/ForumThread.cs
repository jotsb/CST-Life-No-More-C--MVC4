using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class ForumThread
    {
        public ForumThread()
        {
            this.ForumPosts = new List<ForumPost>();
        }

        public int id { get; set; }
        public int subforum_id { get; set; }
        public int author_id { get; set; }
        public string title { get; set; }
        public int num_hits { get; set; }
        public System.DateTime datetime_posted { get; set; }
        public virtual ICollection<ForumPost> ForumPosts { get; set; }
        public virtual Subforum Subforum { get; set; }
        public virtual User User { get; set; }
    }
}

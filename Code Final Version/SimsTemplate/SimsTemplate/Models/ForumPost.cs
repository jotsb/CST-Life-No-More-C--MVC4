using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class ForumPost
    {
        public int id { get; set; }
        public int thread_id { get; set; }
        public int author_id { get; set; }
        public string text { get; set; }
        public System.DateTime datetime_posted { get; set; }
        public virtual ForumThread ForumThread { get; set; }
    }
}

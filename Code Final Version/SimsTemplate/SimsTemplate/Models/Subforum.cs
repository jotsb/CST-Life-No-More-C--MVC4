using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Subforum
    {
        public Subforum()
        {
            this.ForumThreads = new List<ForumThread>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string user_access { get; set; }
        public virtual ICollection<ForumThread> ForumThreads { get; set; }
    }
}

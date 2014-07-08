using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Job
    {
        public Job()
        {
            this.Characters = new List<Character>();
        }

        public int id { get; set; }
        public string value { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Business
    {
        public Business()
        {
            this.Characters = new List<Character>();
        }

        public int id { get; set; }
        public string value { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}

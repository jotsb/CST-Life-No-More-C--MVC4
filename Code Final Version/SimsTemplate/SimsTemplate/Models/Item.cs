using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Item
    {
        public Item()
        {
            this.Characters = new List<Character>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Nullable<int> energy_boost { get; set; }
        public Nullable<int> hunger_boost { get; set; }
        public Nullable<int> sanity_boost { get; set; }
        public Nullable<int> fun_boost { get; set; }
        public Nullable<int> bladder_boost { get; set; }
        public string money_boost { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}

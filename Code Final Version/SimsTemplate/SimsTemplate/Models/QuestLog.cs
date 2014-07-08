using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class QuestLog
    {
        public int char_id { get; set; }
        public int quest_id { get; set; }
        public int current_node { get; set; }
        public Nullable<int> accumulated_value { get; set; }
        public virtual Character Character { get; set; }
        public virtual Quest Quest { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Quest
    {
        public Quest()
        {
            this.QuestLogs = new List<QuestLog>();
        }

        public int id { get; set; }
        public string description { get; set; }
        public int num_nodes { get; set; }
        public virtual ICollection<QuestLog> QuestLogs { get; set; }
    }
}

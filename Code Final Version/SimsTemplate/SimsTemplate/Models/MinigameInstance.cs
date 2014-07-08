using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class MinigameInstance
    {
        public int id { get; set; }
        public int character_id { get; set; }
        public int minigame_id { get; set; }
        public int score { get; set; }
        public virtual Character Character { get; set; }
        public virtual Minigame Minigame { get; set; }
    }
}

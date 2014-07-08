using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Lecture
    {
        public int char_id { get; set; }
        public int minigame_id { get; set; }
        public bool lecture_attended { get; set; }
        public int week { get; set; }
        public virtual Character Character { get; set; }
        public virtual Minigame Minigame { get; set; }
    }
}

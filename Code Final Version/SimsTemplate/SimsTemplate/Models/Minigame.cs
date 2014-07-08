using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class Minigame
    {
        public Minigame()
        {
            this.Lectures = new List<Lecture>();
            this.MinigameInstances = new List<MinigameInstance>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public virtual ICollection<Lecture> Lectures { get; set; }
        public virtual ICollection<MinigameInstance> MinigameInstances { get; set; }
    }
}

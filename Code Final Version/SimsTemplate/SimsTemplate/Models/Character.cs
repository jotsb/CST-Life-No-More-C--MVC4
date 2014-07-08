using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    public class Character
    {
        /// <summary>
        /// @Author Matt Fisher
        /// </summary>
        public Character()
        {
            this.Lectures = new List<Lecture>();
            this.MinigameInstances = new List<MinigameInstance>();
            this.QuestLogs = new List<QuestLog>();
            this.Users = new List<User>();
            this.Items = new List<Item>();
        }

        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public Nullable<int> grades { get; set; }
        public Nullable<int> money { get; set; }
        public string position { get; set; }
        public Nullable<int> inventory { get; set; }
        public Nullable<int> hunger { get; set; }
        public Nullable<int> sanity { get; set; }
        public Nullable<int> fun { get; set; }
        public Nullable<int> energy { get; set; }
        public Nullable<int> bladder { get; set; }
        public Nullable<int> global_score { get; set; }
        public string datetime_ingame { get; set; }
        public Nullable<int> business_level { get; set; }
        public Nullable<int> job_level { get; set; }
        public virtual Business Business { get; set; }
        public virtual Job Job { get; set; }
        public virtual User User { get; set; }
        public virtual CharacterGrade CharacterGrade { get; set; }
        public virtual ICollection<Lecture> Lectures { get; set; }
        public virtual ICollection<MinigameInstance> MinigameInstances { get; set; }
        public virtual ICollection<QuestLog> QuestLogs { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}

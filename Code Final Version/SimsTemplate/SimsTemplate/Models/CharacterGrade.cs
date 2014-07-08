using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class CharacterGrade
    {
        public int char_id { get; set; }
        public int programming_methods { get; set; }
        public int web_development { get; set; }
        public int discrete_mathematics { get; set; }
        public int business_communications { get; set; }
        public virtual Character Character { get; set; }
    }
}

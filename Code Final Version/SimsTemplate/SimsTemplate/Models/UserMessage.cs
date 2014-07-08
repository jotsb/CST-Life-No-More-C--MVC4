using System;
using System.Collections.Generic;

namespace SimsTemplate.Models
{
    /// <summary>
    /// @Author Matt Fisher
    /// </summary>
    public class UserMessage
    {
        public int id { get; set; }
        public int sender_id { get; set; }
        public int recipient_id { get; set; }
        public System.DateTime datetime_sent { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public bool message_read { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}

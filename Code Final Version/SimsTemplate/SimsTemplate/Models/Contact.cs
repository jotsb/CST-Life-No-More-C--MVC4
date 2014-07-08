using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimsTemplate.Models
{
    /// <summary>
    /// Contact Class
    /// 
    /// @Author: Jivanjot Brar
    /// </summary>
    public class Contact
    {

        string TO = "brarjot@hotmail.ca";

        public string To
        {
            get { return TO; }
            set { TO = value; }
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }

    }
}
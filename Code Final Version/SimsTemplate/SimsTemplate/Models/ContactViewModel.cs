using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace SimsTemplate.Models
{
    /// <summary>
    /// Model for the contact form.
    /// 
    /// @Author: Jivanjot Brar
    /// </summary>
    public class ContactViewModel
    {
        [Required(ErrorMessage = "You need to fill in a Name")]
        [DisplayName("Name")]
        [MaxLength(80, ErrorMessage = "Name cannot be longer than 80 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You need to fill in an Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Your Email Address contains some errors")]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You need to enter a Subject")]
        [DisplayName("Subject")]
        [MaxLength(100, ErrorMessage = "Subject cannot be longer than 100 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "You need to fill in a Comment")]
        [DisplayName("Your Comment")]
        public string Comment { get; set; }
    }
}

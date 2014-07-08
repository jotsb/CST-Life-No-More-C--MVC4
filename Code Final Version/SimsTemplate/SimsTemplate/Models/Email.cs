using System;
using System.Net;
using System.Net.Mail;
//using System.Web.Mail;

namespace SimsTemplate.Models
{
    /// <summary>
    /// Email Class
    /// 
    /// @Author: Jivanjot Brar
    /// </summary>
    public class Email
    {

        public void Send(Contact contact)
        {
            MailMessage message = new MailMessage();
            message.To.Add("cstsims2012@gmail.com");
            message.From = new MailAddress(contact.Email);
            message.Subject = contact.Subject;
            message.ReplyToList.Add(contact.Email);
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            string msg = "<table><tr><td>NAME: </td><td>" + contact.Name + "</td></tr><tr><td>E-MAIL ADDRESS: </td><td>" + contact.Email + "</td></tr><tr><td>MESSAGE: </td><td>" + contact.Comment + "</td></tr></table>";
            message.Body = msg;
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("cstsims2012@gmail.com", "techpro2012"),
                EnableSsl = true
            };
            client.Send(message);
        }
    }
}
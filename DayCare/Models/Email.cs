using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DayCare.Models
{
    public class Email
    {
        public void Gmail(string subject, string body, string to)
        {
            string from = "presentationdemos@gmail.com";
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential(from, "7737746@Ps");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mail);

        }
    }
}
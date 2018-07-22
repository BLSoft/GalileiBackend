using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Owin_Auth.Utils;

namespace Galilei.Server.Utils
{
    public class EmailHelper
    {
        public static void SendEmail(string sender,string to,string subject,string content)
        {
            SmtpClient client = new SmtpClient(Config.Configuration["Email:Host"],int.Parse(Config.Configuration["Email:Port"]));
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("noreply@barrow099.hu","Noreply03");
            client.EnableSsl = true;
            


            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@barrow099.hu", sender );
            mailMessage.Subject = subject;
            mailMessage.Body = content;
            mailMessage.To.Add(to);
            client.Send(mailMessage);
        }
    }
}

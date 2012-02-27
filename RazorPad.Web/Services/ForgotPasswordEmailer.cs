using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RazorPad.Web.Services
{
    public interface IForgotPasswordEmailer
    {
        void SendEmail(User user, string resetPasswordUrl);
    }

    public class ForgotPasswordEmailer : IForgotPasswordEmailer
    {
        internal static Action<MailMessage> SendMailThunk = SendMail;

        public void SendEmail(User user, string resetPasswordUrl)
        {
            // TODO: Do this in a cooler way (e.g. Razor templates!)

            var sbMailMsg = new StringBuilder();
            sbMailMsg.AppendFormat("Hi {0},<br /><br />", user.Username);
            sbMailMsg.Append("Please click the below link to reset your password.<br /><br />");
            sbMailMsg.AppendFormat("<a href=\"{0}\">{0}</a>", resetPasswordUrl);
            sbMailMsg.Append("<br /><br />- RazorPad");

            var mailMessage = new MailMessage();
            mailMessage.To.Add(user.EmailAddress);
            mailMessage.Subject = "RazorPad - Password Reset";
            mailMessage.Body = sbMailMsg.ToString();
            mailMessage.IsBodyHtml = true;

            SendMailThunk(mailMessage);
        }

        private static void SendMail(MailMessage mailMessage)
        {
            var smtpClient = new SmtpClient
                                 {
                                     Credentials = new NetworkCredential(
                                         ConfigurationManager.AppSettings["SmtpClient.Username"],
                                         ConfigurationManager.AppSettings["SmtpClient.Password"])
                                 };
            smtpClient.Send(mailMessage);
        }
    }
}

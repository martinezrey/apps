using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Notification
{
    public class NotificationException
    {
        public string Username { get; set; }
        public string Task { get; set; }
        public string Reason { get; set; }
        public IDictionary<string,Object> Parameters { get; set; }
        public string Detail { get; set; }
        public dynamic RawMessage { get; set; }
    }

    public interface INotificationClient
    {
        EventHandler<string> OnFailedToNotify { get; set; }

        void Notify(NotificationException exceptionMessage);
    }

    public class DefaultNotificationClient : INotificationClient
    {
        private static readonly Lazy<DefaultNotificationClient> lazy = new Lazy<DefaultNotificationClient>(() => new DefaultNotificationClient());

        public EventHandler<string> OnFailedToNotify { get; set; }

        private string _subject = ConfigurationManager.AppSettings["ErrorMailSubject"];
        private string _from = ConfigurationManager.AppSettings["MailFrom"];
        private string _admin = ConfigurationManager.AppSettings["MailToAdmin"];
        private string _mailServer = ConfigurationManager.AppSettings["MailServer"];

        public static DefaultNotificationClient Instance { get { return lazy.Value; } }

        public void Notify(StringBuilder message)
        {
            // send error to admin
            MailMessage mail = new MailMessage();

            mail.Subject = _subject;

            mail.Body = message.ToString().Replace("\\r\\n", "<br />");

            mail.From = new MailAddress(_from);

            string mailTo = _admin;

            if (mailTo != null & mailTo.Trim().Length > 0)
            {
                string[] emails = mailTo.Split(";".ToCharArray());
                foreach (string email in emails)
                {
                    mail.To.Add(new MailAddress(email));
                }
            }

            mail.IsBodyHtml = true;

            SmtpClient smtp = null;

            try
            {
                smtp = new SmtpClient(_mailServer);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                if (OnFailedToNotify != null)
                    OnFailedToNotify(this, JsonConvert.SerializeObject(new 
                    {
                        dateThrown = DateTime.Now,
                        exception = ex,
                        notificationMessage = message.ToString(),
                    }));
            }
            finally
            {
                if (smtp != null)
                    smtp.Dispose();
            }
        }

        public void Notify(NotificationException exceptionMessage)
        {
            StringBuilder body = new StringBuilder();

            body.Append("<b>User:</b> " + exceptionMessage.Username + "<br />");
            body.Append("<br /><b>Task:</b> " + exceptionMessage.Task + "<br />");
            body.Append("<br /><b>Reason:</b> " + exceptionMessage.Reason + "<br />");

            body.Append("<br /><b>Parameters:</b> <br />");

            foreach (var param in exceptionMessage.Parameters)
            {
                 body.Append(String.Format("{0}: {1}<br />", param.Key, JsonConvert.SerializeObject(param.Value)));
            }

            body.Append("<br /><b>Detail:</b> " + exceptionMessage.Detail + "<br />");
            body.Append("<br /><b>Raw Details:</b> " + JsonConvert.SerializeObject(exceptionMessage.RawMessage) + "<br />");

            Notify(body);
        }
    }
}

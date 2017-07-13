using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using NLog;
using TradingPlatform.Data;
using TradingPlatform.Enums;

namespace TradingPlatform.Models
{

    public static class Mailer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string _Alias = "mail@ptp.ua";
        private static readonly string _Domain = "kyiv.onmicrosoft.com";
        private static readonly string _Password = "V'h,Z!3vS&*7(y,g";
        private static readonly string _UserName = "ptpinfo@kyiv.onmicrosoft.com";
        private static readonly string _smtp = "smtp.office365.com";
        private static readonly int _port = 587;

        public static void SendMailWorker(string address, string subject, string body)
        {

            MailAddress from = new MailAddress(_Alias);
            MailAddress to = new MailAddress(address);
            MailMessage m = new MailMessage(from, to);

            m.Subject = subject;
            m.Body = "<meta charset='utf-8'>" + body;
            m.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(_smtp, _port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential()
                {
                    Domain = _Domain,
                    Password = _Password,
                    UserName = _UserName
                }
            };
         
            try
            {
                logger.Info($"sending mail to {to.Address}");

                smtp.Send(m);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }
        //public static void SendMail(string address, string subject, string body)
        //{
        //    CreateAndSendNotification(address, subject, body);
        //}

        public static void SendFeedback(string address, string subject, string body)
        {
            SendMailWorker(address, subject, body);
        }

      
        public static void CreateAndSendNotification(string address, string subject, string body)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var touser = db.Users.FirstOrDefault(c => c.Email == address);

                    var item = new Notification
                    {
                        Subject = subject,
                        Body = body,
                        Email = address,
                        CreateDate = DateTime.Now,
                        IsViewed = false,
                        ViewedDate = DateTime.Now,
                        ToUser = touser
                    };
                    logger.Info($"Notification for {address} created. Total count {StaticData.Notifications.Count}");
                    StaticData.Notifications.Add(item);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }
        
       
    }
}

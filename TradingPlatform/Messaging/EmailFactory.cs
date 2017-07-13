using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Messaging
{
    public static class EmailFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string _Alias = "";
        private static readonly string _Domain = "";
        private static readonly string _Password = "";
        private static readonly string _UserName = "";
        private static readonly string _smtp = "";
        private static readonly int _port = 000;
        private static SmtpClient _client;
        private static SmtpClient SmtpClient => _client ?? (_client = new SmtpClient()
        {
            Credentials = new NetworkCredential
            {
                Domain = _Domain,
                Password = _Password,
                UserName = _UserName
            },
            Host = _smtp,
            Port = _port,
            EnableSsl = true
        });

        public static void SendEmail(string to, string subject, string body)
        {
            Thread email = new Thread(delegate ()
            {
                SendAsyncEmail(to, subject, body);
            });
            email.IsBackground = true;
            email.Start();
        }

        private static void SendAsyncEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                if (_smtp != null)
                {
                    client.Host = _smtp;
                    client.Port = Convert.ToInt32(_port);
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential(_UserName, _Password, _Domain);
                }
                message.To.Add(to);
                message.From = new MailAddress(_Alias, "Petroleum Trading Platform");
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                client.Send(message);

                logger.Info($"mail {subject} to {to} sended");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "SMTP exception");
                logger.Info($"NO mail {subject} to {to} notsended");
                Thread.Sleep(30000);
                SendAsyncEmail(to, subject, body);
            }
        }



        public static void SendEmailAsync(BaseNotify not)
        {
            if (not.Send)
            {
                var email = new Email(not);
                SendEmail(email.ToEmail, "[PTP]:" + email.Subject, email.Message);
            }

        }
        public static void SendEmailAsync(Broadcast broadcast)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var userManger = new ApplicationUserManager(new ApplicationUserStore(db));
                var user = userManger.FindByEmailAsync(broadcast.Email).Result;
                if (user!=null)
                {
                    SendEmailAsync(broadcast, user);
                }
                else
                {
#if DEBUG
                    return;
#endif
                    var email = new Email(broadcast);
                    SendEmail(email.ToEmail, "[PTP]:" + email.Subject, email.Message);

                }
            }


        }
        public static void SendEmailAsync(Broadcast broadcast, ApplicationUser user)
        {
            if (IsSendBroadcast(user))
            {
                broadcast.Email = user.Email;
                broadcast.UserName = user.RegisterName;
                var email = new Email(broadcast);
                SendEmail(email.ToEmail, "[PTP]:" + email.Subject, email.Message);
            }


        }
        public static bool IsSendBroadcast(ApplicationUser user)
        {
#if DEBUG
            return user.IsDebug;
#else
            return user.AllowPromoEmails; //&&this.Promo||user.AllowTradeEmails&&Trade||Important;
#endif
        }

        //public static void SendEmailAsync(Broadcast email)
        //{

        //    var body = RenderViewToString("~/Views/Email/Broadcast.cshtml", email);
        //       // var email = new Email(not);
        //        SendEmail(email.Address, "[PTP]:" + email.Subject, body);


        //}


        public static string RenderViewToString(string viewPath, object model, bool partial = false, ViewDataDictionary viewDataDictionary = null)
        {
            try
            {
                if (!viewPath.Contains("~/Views/Email"))
                {
                    viewPath = "~/Views/Email/" + viewPath + ".cshtml";
                }

                // first find the ViewEngine for this view
                ViewEngineResult viewEngineResult = null;
                if (partial)
                    viewEngineResult = ViewEngines.Engines.FindPartialView(FakeControllerContext, viewPath);
                else
                    viewEngineResult = ViewEngines.Engines.FindView(FakeControllerContext, viewPath, null);

                if (viewEngineResult == null)
                    throw new FileNotFoundException("View cannot be found.");

                if (viewDataDictionary == null)
                    viewDataDictionary = new ViewDataDictionary();

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                viewDataDictionary.Model = model;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(FakeControllerContext, view, viewDataDictionary, FakeControllerContext.Controller.TempData, sw);
                    view.Render(ctx, sw);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                var t = ex;
                throw;
            }
        }

        public static async Task Brodcast(Broadcast model)
        {
            await Telegram.Broadcast(model);
            var noncorrectids = await Push.Broadcast(model);
            using (var db = new ApplicationDbContext())
            {
                if (noncorrectids != null && noncorrectids.Any())
                {
                    foreach (var deviceid in noncorrectids)
                    {
                        db.Pushes.Remove(db.Pushes.FirstOrDefault(c => c.DeviceId == deviceid));
                    }
                    db.SaveChanges();
                }

            }
        }

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "https://ptp.ua/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, CallingConventions.Standard,
                    new[] { typeof(HttpSessionStateContainer) },
                    null)
                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }

        private static HttpContext Ctx => HttpContext.Current ?? FakeHttpContext();


        private static ControllerContext _fakeControllerContext;
        private static ControllerContext FakeControllerContext
        {
            get
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", "Fake");
                _fakeControllerContext = new ControllerContext(new HttpContextWrapper(Ctx), routeData, new FakeController());
                return _fakeControllerContext;
            }
        }
        private class FakeController : ControllerBase { protected override void ExecuteCore() { } }
    }
}

using NLog;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Quartz;
using Quartz.Impl;
using TradingPlatform.Controllers;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Http;
using TradingPlatform.Models;
using TradingPlatform.Messaging;

namespace TradingPlatform
{
    public class MvcApplication : System.Web.HttpApplication
    {
       // private static Logger logger = LogManager.GetCurrentClassLogger();
     
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["UsersOnline"] = 0;
            ShedulerTest();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory());
        }
       
        //public void TelegramCheck()
        //{
        //    TradingPlatform.Messaging.Telegram.Init();
        //}
       
        public void ShedulerTest()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            
            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();
            
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<Jobs.CheckNewTradesJob>()
                .WithIdentity("myJob", "group1")
                .Build();
            
            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow().WithSimpleSchedule(c=>c.WithIntervalInSeconds(30).RepeatForever()).ForJob("myJob", "group1")
              //.WithSimpleSchedule(x => x
              //    .WithIntervalInSeconds(30)
              //    .RepeatForever())
              .Build();
            
            sched.ScheduleJob(job, trigger);
        }
        protected void Session_Start()
        {
            Application.Lock();
            
            //Logger logger = LogManager.GetCurrentClassLogger();
            //logger.Info("session started");
            Application["UsersOnline"] = (int)Application["UsersOnline"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();
           
            //var auth = Request.GetOwinContext().Authentication;
            //auth.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Application["UsersOnline"] = (int)Application["UsersOnline"] - 1;
            Application.UnLock();
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpCookie cultureCookie = HttpContext.Current.Request.Cookies.Get("language");
            if (cultureCookie == null)
            {
                cultureCookie = LocalText.CreateCookie();
                HttpContext.Current.Response.Cookies.Add(cultureCookie);
            }
        }

        protected void Application_Error()
        {
            Logger log = LogManager.GetCurrentClassLogger();
            Exception exception = Server.GetLastError();
            log.Error(exception);
           // Server.ClearError();
            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Index");
            }
            else //It's an Http Exception, Let's handle it.
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "HttpError404");
                        break;
                    case 500:
                        // Server error.
                        routeData.Values.Add("action", "HttpError500");
                        break;

                    // Here you can handle Views to other error codes.
                    // I choose a General error template  
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }
            }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);
            
            // Clear the error on server.
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
            
            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }
    }
}

using System;
using System.Linq;

using System.Web.Mvc;
using PerpetuumSoft.Knockout;


namespace TradingPlatform.Controllers
{
   
    public class BaseController : KnockoutController, IDisposable
    {
             
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                     
            if (HttpContext.Request.Cookies.AllKeys.Contains("timezoneoffset"))
            {
               Session["timezoneoffset"] = HttpContext.Request.Cookies["timezoneoffset"].Value;
            }
            if (HttpContext.Request.Cookies.AllKeys.Contains("timediff"))
            {
                Session["timediff"] = HttpContext.Request.Cookies["timediff"].Value;
            }
            ViewBag.Lang = LocalText.GetCultureName();
            base.OnActionExecuting(filterContext);
        }

        public static string GetFullErrorMessage(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += "; " + GetFullErrorMessage(ex.InnerException);
            }
            return message;
        }

    }
}
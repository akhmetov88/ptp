using System.Linq;
using System.Web.Mvc;

namespace TradingPlatform.Models
{
    public class InLeftMenuAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.Controller.ViewBag.MenuAnchor = (filterContext.RouteData.DataTokens["RouteName"] as string).Split('/').FirstOrDefault();
            }
            catch {}

            base.OnActionExecuting(filterContext);
        }
    }
}

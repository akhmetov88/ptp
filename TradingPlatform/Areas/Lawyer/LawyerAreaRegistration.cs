using System.Web.Mvc;

namespace TradingPlatform.Areas.Lawyer
{
    public class LawyerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Lawyer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.LowercaseUrls = true;
            context.MapRoute(
                "Lawyer_default",
                "Lawyer/{controller}/{action}/{id}",
                new { action = "Index", controller = "Contragent", id = UrlParameter.Optional }
            );
        }
    }
}
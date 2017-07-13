using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using TradingPlatform.Data;
using System.Web.Http;

using TradingPlatform.Models;
using System.Net.Http.Headers;

namespace TradingPlatform
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //  config.MapHttpAttributeRoutes();
    
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Push>("Push");
            builder.EntitySet<ApplicationUser>("ApplicationUsers");
            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            //config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}

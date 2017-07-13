using System.Web.Mvc;
using System.Web.Routing;

namespace TradingPlatform
{
    public static class RouteCollectionExtensions
    {
        public static Route MapRouteWithName(this RouteCollection routes,
        string name, string url, object defaults, object constraints = null)
        {
            Route route = routes.MapRoute(name, url, defaults, constraints);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens.Add("RouteName", name);

            return route;
        }
    }
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RegisterLeftMenuRoutes(routes);

            routes.MapRouteWithName(
               name: "PrivacyPolicy",
               url: "privacypolicy/{id}",
               defaults: new { controller = "Home", action = "PrivacyPolicy", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
             name: "t",
             url: "t",
             defaults: new { controller = "Home", action = "t" }
          );
            routes.MapRouteWithName(
           name: "settings",
           url: "settings",
           defaults: new { controller = "Account", action = "EditProfile" }
        );
            routes.MapRouteWithName(
                name: "my",
                url: "my/{id}",
                defaults: new { controller = "Trade", action = "My", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
               name: "About",
               url: "about/{id}",
               defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
               name: "Documents",
               url: "documents/{id}",
               defaults: new { controller = "Home", action = "Documents", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
               name: "FAQ",
               url: "faq/{id}",
               defaults: new { controller = "Home", action = "FAQ", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
            name: "therms",
            url: "therms/{id}",
            defaults: new { controller = "Home", action = "Therms", id = UrlParameter.Optional }
         );

            routes.MapRouteWithName(
             name: "chat",
             url: "chat/{id}",
             defaults: new { controller = "trade", action = "chat", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "robots",
              url: "robots.txt",
              defaults: new { controller = "Home", action = "Robots" }
          );
            routes.MapRoute(
                     name: "sitemap",
                     url: "sitemap.xml",
                     defaults: new { controller = "Home", action = "SiteMap" }
                 );
            routes.MapRouteWithName(
            name: "uk",
            url: "uk",
            defaults: new { controller = "Home", action = "Uk" }
         );
            routes.MapRouteWithName(
          name: "ru",
          url: "ru",
          defaults: new { controller = "Home", action = "Ru" }
       );

            routes.MapRouteWithName(
               name: "HowItUse",
               url: "howituse/{id}",
               defaults: new { controller = "Home", action = "HowItUse", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
               name: "AccreditationRules",
               url: "accreditationrules/{id}",
               defaults: new { controller = "Home", action = "AccreditationRules", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
               name: "Feedback",
               url: "feedback/{id}",
               defaults: new { controller = "Home", action = "Feedback", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
                name: "Contacts",
                url: "contacts/{id}",
                defaults: new { controller = "Home", action = "Contacts", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
               name: "Rules",
               url: "rules/{id}",
               defaults: new { controller = "Home", action = "rules", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
               name: "Lang",
               url: "ChangeLanguage/{id}",
               defaults: new { controller = "Home", action = "ChangeLanguage", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
                name: "DefaultWebApi",
                url: "api/{controller}/{id}",
                defaults: new { id = UrlParameter.Optional });
            routes.MapRouteWithName(
                name: "OdataApi",
                url: "odata/{controller}/{id}",
                defaults: new { id = UrlParameter.Optional });
        }


        private static void RegisterLeftMenuRoutes(RouteCollection routes)
        {



            // MapRouteWithName сохраняет в Route.DataTokens имя (name)
            // оно используется для подсвета левого меню, 
            // для этого надо к ActionResult подсавить фильтр [InLeftMenu]
            routes.MapRouteWithName(
              name: "auction-buyer-future",
              url: "auction-buyer-future",
              defaults: new { controller = "Trade", action = "Index", filter = "future" }
           );
            routes.MapRouteWithName(
          name: "approving",
          url: "approving",
          defaults: new { controller = "Trade", action = "TradesOnApproving", id = UrlParameter.Optional }
           );


            routes.MapRouteWithName(
            name: "auction-buyer",
            url: "auction-buyer",
            defaults: new { controller = "Trade", action = "BuyerTrades" }
             );
            routes.MapRouteWithName(
           name: "auction-buyer-documents",
           url: "auction-buyer-documents",
           defaults: new { controller = "Trade", action = "BuyerDocs" }
        );
            routes.MapRouteWithName(
              name: "auction-seller-documents",
              url: "auction-seller-documents/{tradeId}/{buyerId}",
              defaults: new { controller = "Trade", action = "SellerDocs", tradeId = UrlParameter.Optional, buyerId = UrlParameter.Optional }
           );
            routes.MapRouteWithName(
                name: "auction-seller",
                url: "auction-seller",
                defaults: new { controller = "Trade", action = "SellerTrades" }
             );

            routes.MapRouteWithName(
               name: "auction-seller-history",
               url: "auction-seller-history",
               defaults: new { controller = "Trade", action = "TradesHistory" }
            );

            routes.MapRouteWithName(
                 name: "auction-buyer-current",
                 url: "auction-buyer-current",
                 defaults: new { controller = "Trade", action = "Index", filter = "actual" }
              );
            routes.MapRouteWithName(
                 name: "auction-buyer-past",
                 url: "auction-buyer-past",
                 defaults: new { controller = "Trade", action = "Index", filter = "past" }
              );

            routes.MapRouteWithName(
                name: "user-setings-profile",
                url: "user-setings-profile",
                defaults: new { controller = "Account", action = "EditProfile" }
            );


            routes.MapRouteWithName(
               name: "user-setings-password",
               url: "user-setings-password",
               defaults: new { controller = "Manage", action = "ChangePassword" }
            );
            routes.MapRouteWithName(
               name: "user-setings-entities",
               url: "user-setings-entities/{id}",
               defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRouteWithName(
              name: "auction-seller-create",
              url: "auction-seller-create/{id}",
              defaults: new { controller = "Trade", action = "CreateTrade", id = UrlParameter.Optional }
            );
            routes.MapRouteWithName(
      name: "sendmails",
      url: "sendmails",
      defaults: new { controller = "Home", action = "SendMails",area="" }
   );
            routes.MapRouteWithName(
              name: "user-setings-entities/edit",
              url: "user-setings-entities/edit/{id}",
              defaults: new { controller = "Account", action = "Edit", id = UrlParameter.Optional }
            );

            #region lawyer

            routes.MapRouteWithName(
               name: "administration-feedback",
               url: "administration-feedback/{id}",
               defaults: new { controller = "Feedback", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");

            routes.MapRouteWithName(
               name: "administration-faq",
               url: "administration-faq/{action}/{id}",
               defaults: new { controller = "FAQ", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");

            routes.MapRouteWithName(
               name: "administration-request",
               url: "administration-request/{id}",
               defaults: new { controller = "Contragent", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Lawyer");

            routes.MapRouteWithName(
               name: "administration-request-history",
               url: "administration-request-history/{id}",
               defaults: new { controller = "Contragent", action = "History", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Lawyer");

            routes.MapRouteWithName(
               name: "administration-request-details",
               url: "administration-request-details/{id}",
               defaults: new { controller = "Contragent", action = "Details", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Lawyer");
            #endregion

            #region
            routes.MapRouteWithName(
               name: "administration-content",
               url: "administration-content/{id}",
               defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");

            routes.MapRouteWithName(
               name: "administration-help",
               url: "administration-help/{id}",
               defaults: new { controller = "Help", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");

            routes.MapRouteWithName(
             name: "directory",
             url: "directory/{id}",
             defaults: new { controller = "Directory", action = "Edit", id = UrlParameter.Optional }
          ).DataTokens.Add("area", "Admin");

            routes.MapRouteWithName(
               name: "administration-email",
               url: "administration-email/{id}",
               defaults: new { controller = "EmailTemplate", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");
            routes.MapRouteWithName(
               name: "administration-tocken",
               url: "administration-tocken/{id}",
               defaults: new { controller = "Token", action = "Index", id = UrlParameter.Optional }
            ).DataTokens.Add("area", "Admin");
            #endregion
        }
    }
}

using System.Web.Optimization;

namespace TradingPlatform
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-*",
                       "~/Scripts/jquery.cookie-1.4.1.min.js",
                       "~/Scripts/jquery.countdown.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/jquery.signalR-2.2.0.min.js",
                        "~/Scripts/notify/notify.js",
                        "~/Scripts/notify/styles/metro/notify-metro.js",
                        "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                "~/scripts/dt/bootstrap-datetimepicker.min.js",
                "~/scripts/dt/locales/bootstrap-datetimepicker.uk.js", 
                "~/scripts/dt/locales/bootstrap-datetimepicker.ru.js"
                ));

            bundles.Add(new StyleBundle("~/bundle/css").Include(
                       "~/Content/normalize.css",
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/Modal.css",
                      "~/Content/font-awesome.min.css",
                      "~/Scripts/notify/styles/metro/notify-metro.css"));

            BundleTable.EnableOptimizations = true;

        }
    }
}

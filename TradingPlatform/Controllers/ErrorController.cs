using System.Web.Mvc;

namespace TradingPlatform.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 501;
            return View("General");
        }
        public ActionResult General()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult HttpError404()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult HttpError500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
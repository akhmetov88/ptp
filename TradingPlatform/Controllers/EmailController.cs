using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Models.ProfileModel;

namespace TradingPlatform.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index(string id)
        {
            var model = new UserViewModel() { Id = "42", UserName = id, Email = id };
            return View(model);
        }
    }
}
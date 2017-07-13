using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using TradingPlatform.Models;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Areas.Admin.Controllers
{
    [Authorize(Roles = "root, admin")]
    public class FeedbackController : Controller
    {
        private ApplicationDbContext _db;
        //    TeleAuth telegrams = new TeleAuth();

        public ApplicationDbContext db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }

        [InLeftMenu]
        public ActionResult Index(bool isCommited = false)
        {
            ViewBag.IsCommited = isCommited;
            var model = db.Feedbacks.AsQueryable();

            if (isCommited)
                model = model.Where(x => x.IsCommited == false);

            return View(model.OrderByDescending(x => x.Date).ToList());
        }

        public ActionResult Reply(int id)
        {
            var item = db.Feedbacks.SingleOrDefault(x => x.Id == id);
            if (item==null)
            {
                return HttpNotFound();
            }
            return View("~/Views/Home/SendMails.cshtml", new Broadcast(item));
        }


        public ActionResult Commit(int id)
        {
            var item = db.Feedbacks.SingleOrDefault(x => x.Id == id);
            if(item != null)
            {
                item.IsCommited = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return Json(null);
        }
    }
}
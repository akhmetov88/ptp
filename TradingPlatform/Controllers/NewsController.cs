using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Data;
using TradingPlatform.Models;

namespace TradingPlatform.Controllers
{
    public class NewsController : Controller
    {
        #region var
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db;
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
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        #endregion

        // GET: News
        public ActionResult Index()
        {
            var news = db.News.Include(t => t.Trade);
            return View(news.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New model = db.News.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: News/Create
        public ActionResult Create()
        {

           // var t = new SelectList(new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "Notrade" } });
            
          //  ViewBag.TradeId
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,Link")] New model)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           // ViewBag.TradeId = new SelectList(db.Trades, "Id", "Type", model.TradeId);
            return View(model);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New model = db.News.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            //ViewBag.TradeId = new SelectList(db.Trades, "Id", "Type", model.TradeId);
            return View(model);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Link")] New model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.TradeId = new SelectList(db.Trades, "Id", "Type", model.TradeId);
            return View(model);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New model = db.News.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            New model = db.News.Find(id);
            db.News.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradingPlatform.Data;
using TradingPlatform.Models;

namespace TradingPlatform.Controllers
{
    public class QuotationsController : Controller
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

        // GET: Quotations
        public ActionResult Index()
        {
            var quotations = db.Quotations.Include(q => q.CreatedByUser).Include(q => q.QuotationType).Include(q => q.UpdatedByUser);
            return View(quotations.ToList());
        }

        // GET: Quotations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // GET: Quotations/Create
        public ActionResult Create()
        {
           // ViewBag.CreatedByUserId = new SelectList(db.Users, "Id", "RegisterName");
            ViewBag.QuotationTypeId = new SelectList(db.QuotationTypes, "Id", "Type");
            //ViewBag.UpdatedByUserId = new SelectList(db.Users, "Id", "RegisterName");
            return View();
        }

        // POST: Quotations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Value,Date,QuotationTypeId")] Quotation quotation)
        {
            if (ModelState.IsValid)
            {
                db.Quotations.Add(quotation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuotationTypeId = new SelectList(db.QuotationTypes, "Id", "Type", quotation.QuotationTypeId);
            return View(quotation);
        }

        // GET: Quotations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuotationTypeId = new SelectList(db.QuotationTypes, "Id", "Type", quotation.QuotationTypeId);
            return View(quotation);
        }

        // POST: Quotations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Date,QuotationTypeId")] Quotation model)
        {

            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var quotation = db.Quotations.Find(model.Id);
                quotation.QuotationTypeId = model.QuotationTypeId;
                quotation.Value = model.Value;
                quotation.Date = model.Date;
                db.UpdateEntity(quotation, user.Id);               
                return RedirectToAction("Index");
            }
            ViewBag.QuotationTypeId = new SelectList(db.QuotationTypes, "Id", "Type", model.QuotationTypeId);
            return View(model);
        }

        // GET: Quotations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // POST: Quotations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quotation quotation = db.Quotations.Find(id);
            db.Quotations.Remove(quotation);
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

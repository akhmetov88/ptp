using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
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
    public class QuotationTypesController : Controller
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

        // GET: QuotationTypes
        public ActionResult Index()
        {
            var quotationTypes = db.QuotationTypes.Include(q => q.CreatedByUser).Include(q => q.UpdatedByUser);
            return View(quotationTypes.ToList());
        }

        // GET: QuotationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuotationType quotationType = db.QuotationTypes.Find(id);
            if (quotationType == null)
            {
                return HttpNotFound();
            }
            return View(quotationType);
        }

        // GET: QuotationTypes/Create
        public ActionResult Create()
        {          
            return View();
        }

        // POST: QuotationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] QuotationType quotationType)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                db.Insert(quotationType, user.Id);
                return RedirectToAction("Index");
            }          
            return View(quotationType);
        }

        // GET: QuotationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuotationType quotationType = db.QuotationTypes.Find(id);
            if (quotationType == null)
            {
                return HttpNotFound();
            }
         
            return View(quotationType);
        }

        // POST: QuotationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] QuotationType model)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                var quotationType = db.QuotationTypes.Find(model.Id);
                quotationType.Type = model.Type;
                db.UpdateEntity(quotationType, user.Id);             
                return RedirectToAction("Index");
            }
         
            return View(model);
        }

        // GET: QuotationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuotationType quotationType = db.QuotationTypes.Find(id);
            if (quotationType == null)
            {
                return HttpNotFound();
            }
            return View(quotationType);
        }

        // POST: QuotationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuotationType quotationType = db.QuotationTypes.Find(id);
            db.QuotationTypes.Remove(quotationType);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Models;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Controllers.api
{
    public class ContentsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Catalogs
        public IEnumerable<ContentView> GetContents()
        {
            return db.ContentPages.ToList().Select(c=>new ContentView(c)).ToList();//.Select(c=>new CatalogModel(c)).ToList();
        }

        // GET: api/Catalogs/5
        [ResponseType(typeof(Token))]
        public IHttpActionResult GetContent(int id)
        {
            var catalog = db.ContentPages.Find(id);
            if (catalog == null)
            {
                return NotFound();
            }

            return Ok(catalog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CatalogExists(int id)
        {
            return db.ContentPages.Count(e => e.Id == id) > 0;
        }
    }
}
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

namespace TradingPlatform.Controllers.api
{
    public class CatalogsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Catalogs
        public IEnumerable<CatalogModel> GetCatalogs()
        {
            return db.Catalogs.OrderBy(c=>c.Id).ToList().Select(c=>new CatalogModel(c,"")).ToList();
        }

        // GET: api/Catalogs/5
        [ResponseType(typeof(Catalog))]
        public IHttpActionResult GetCatalog(int id)
        {
            Catalog catalog = db.Catalogs.Find(id);
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
            return db.Catalogs.Count(e => e.Id == id) > 0;
        }
    }
}
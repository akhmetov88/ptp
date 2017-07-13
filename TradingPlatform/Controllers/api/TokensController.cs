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
    public class TokensController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Catalogs
        public IEnumerable<TokenModel> GetTokens()
        {
            return db.Tokens.ToList().Select(c=>new TokenModel(c)).ToList();
        }

        // GET: api/Catalogs/5
        [ResponseType(typeof(Token))]
        public IHttpActionResult GetToken(int id)
        {
            Token catalog = db.Tokens.Find(id);
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
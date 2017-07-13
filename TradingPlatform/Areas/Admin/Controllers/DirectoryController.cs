using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TradingPlatform.Models;
using PerpetuumSoft.Knockout;
using TradingPlatform.Areas.Admin.Models;
using TradingPlatform.Data;
using TradingPlatform.Enums;
using System.Net;
using Newtonsoft.Json;
using TradingPlatform.Models.ViewModel;

namespace TradingPlatform.Areas.Admin.Controllers
{
    //[Authorize(Roles = "root, admin")]
    [AllowAnonymous]
    public class DirectoryController : KnockoutController
    {

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
        private EditCatalogModel CreateEditModel()
        {
            var tt = db.Catalogs.ToList();
            var t = db.Catalogs.Where(c => c.ParentId == null).ToList().Select(c => new CatalogModel(c,true)).ToList();
            return new EditCatalogModel()
            {
                Catalogs = db.Catalogs.Where(c => c.ParentId == null).ToList().Select(c => new CatalogModel(c,true)).ToList(),
                Types = Enum.GetNames(typeof(CatalogType)).ToList()
            };
        }
      


        private CatalogModel Find(List<CatalogModel> models, int id)
        {
            CatalogModel result = null;

            foreach (var catalog in models)
            {
                if (catalog.Id == id)
                {
                    return catalog;
                }
                if (catalog.Dependencies == null) continue;
                result = Find(catalog.Dependencies, id);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }
        [AllowAnonymous]
        public ActionResult Edit()
        {
            return View(CreateEditModel());
        }
        [AllowAnonymous]
        //public ActionResult Test()
        //{
        //    return View(CreateEditModel());
        //}
        public ActionResult AddCatalog(EditCatalogModel model)
        {
            var catalog = new Catalog()
            {
                Code = Guid.NewGuid().ToString(),
                Ru = "Справочник",
                Uk = "Довідник",
                DescRu = "",
                DescUk = "",
                UserId = User.Identity.GetUserId() ?? null,
            };
            db.Insert(catalog, User.Identity.GetUserId());
            StaticData.Inst.Update(db);
            model.AddRoot(catalog);
            return Json(model);


        }
        public ActionResult AddDependency(EditCatalogModel model, int parentId)
        {

            var dependency = new Catalog()
            {
                Code = Guid.NewGuid().ToString(),
                Ru = "Зависимость",
                Uk = "Залежність",
                DescRu = "",
                DescUk = "",
                ParentId = parentId,
                UserId = User.Identity.GetUserId() ?? null,

            };

            db.Insert(dependency, User.Identity.GetUserId());
            StaticData.Inst.Update(db);
            return Json(CreateEditModel());

        }
        public ActionResult Delete(EditCatalogModel model, int modelId)
        {
            CatalogModel todel = Find(model.Catalogs, modelId);
            if (todel != null)
            {
                model.Delete(todel, db);
            }
            StaticData.Inst.Update(db);
            return Json(model);
        }

        public ActionResult Save(EditCatalogModel model, int modelId)
        {
            CatalogModel tosave = Find(model.Catalogs, modelId);
            if (tosave != null)
            {
                model.Save(tosave, db, User.Identity.GetUserId());
            }
            StaticData.Inst.Update(db);
            return Json(model);
        }







    }
}


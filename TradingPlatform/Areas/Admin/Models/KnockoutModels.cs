using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.Data;
using TradingPlatform.Models;

namespace TradingPlatform.Areas.Admin.Models
{

    public class EditCatalogModel
    {
        public List<CatalogModel> Catalogs { get; set; }
        public List<string> Types { get; set; }
        public string SelectedType { get; set; }
        public bool redirect
        {
            get { return true; }
            set { }
        }

        public string url
        {
            get
            {
                return "/directory";
            }
            set
            {
            }
        }
        public void AddRoot(Catalog catalog)
        {
            Catalogs.Add(new CatalogModel(catalog));
        }
        //public void AddDependency(Catalog dependency,int parentId)
        //{
        //    Catalogs.SingleOrDefault(c=>c.Id==parentId).Dependencies.Add(new CatalogModel(dependency));
        //}

        public void Save(CatalogModel model, ApplicationDbContext db, string userId = null)
        {
            var catalog = db.Catalogs.SingleOrDefault(c => c.Id == model.Id);
            catalog.Ru = model.ru;
            catalog.Uk = model.uk;
            catalog.Code = model.Code;
            catalog.DescRu = model.DescRu;
            catalog.DescUk = model.DescUk;
            catalog.Type = model.Type;
            catalog.UserId = userId;
            catalog.IsUsable = model.IsUsable;
            catalog.Filter = model.Filter;
            db.UpdateEntity(catalog, userId);
        }

        public void Delete(CatalogModel model, ApplicationDbContext db)
        {
            var catalog = db.Catalogs.SingleOrDefault(c => c.Id == model.Id);
            db.Catalogs.Remove(catalog);
            db.SaveChanges();
        }
    }


    public class CatalogModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="isusable">Грузить ли не юзабельные справочники</param>
        public CatalogModel(Catalog catalog, bool isusable)
        {
            Id = catalog.Id;
            Code = catalog.Code;
            DescRu = catalog.DescRu;
            DescUk = catalog.DescUk;
            Id = catalog.Id;
            ParentId = catalog.ParentId;
            ru = catalog.Ru;
            uk = catalog.Uk;
            CanDelete = !catalog.Dependencies.Any();
            Type = catalog.Type;
            IsUsable = catalog.IsUsable;
            Filter = catalog.Filter;
            Updated = catalog.Updated;
            Dependencies = catalog.Dependencies.ToList().Select(deps => new CatalogModel(deps, true)).ToList();
        }

        public CatalogModel(Catalog catalog)
        {
            Code = catalog.Code;
            DescRu = catalog.DescRu;
            DescUk = catalog.DescUk;
            Filter = catalog.Filter;
            Id = catalog.Id;
            ParentId = catalog.ParentId;
            ru = catalog.Ru;
            uk = catalog.Uk;
            CanDelete = !catalog.Dependencies.Any();
            Type = catalog.Type;
            IsUsable = catalog.IsUsable;
            Updated = catalog.Updated;
            Dependencies = catalog.Dependencies.Where(c=>c.IsUsable).Select(deps => new CatalogModel(deps)).ToList();
        }
        public CatalogModel(Catalog catalog, string update)
        {
            Code = catalog.Code;
            DescRu = catalog.DescRu;
            DescUk = catalog.DescUk;
            Filter = catalog.Filter;
            Id = catalog.Id;
            ParentId = catalog.ParentId;
            ru = catalog.Ru;
            uk = catalog.Uk;
            CanDelete = !catalog.Dependencies.Any();
            Type = catalog.Type;
            IsUsable = catalog.IsUsable;
            Updated = catalog.Updated;
            //Dependencies = catalog.Dependencies.Where(c => c.IsUsable).Select(deps => new CatalogModel(deps)).ToList();
        }

        public CatalogModel()
        {

        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string uk { get; set; }
        public string ru { get; set; }
        public string Filter { get; set; }
        public string DescUk { get; set; }
        public string DescRu { get; set; }
        public bool CanDelete { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
        public bool IsUsable { get; set; }
        public DateTime Updated { get; set; }
        public List<CatalogModel> Dependencies { get; set; }
    }



}

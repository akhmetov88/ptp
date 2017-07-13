using System;
using System.Collections.Generic;
using TradingPlatform.Areas.Admin.Models;

namespace TradingPlatform.Data
{

    public partial class Catalog : BaseEntity
    {
        public Catalog()
        {
           // Dependencies = new HashSet<Catalog>();
        }
        public Catalog(CatalogModel catalog)
        {
            Code = catalog.Code??"";
            DescRu = catalog.DescRu??"";
            DescUk = catalog.DescUk ?? "";
            Id = catalog.Id;
            ParentId = catalog.ParentId??null;
            Ru = catalog.ru;
            Uk = catalog.uk;
            Type = catalog.Type;
            IsUsable = catalog.IsUsable;
            Filter = catalog.Filter;
            Updated = catalog.Updated;
        }

        public string Code { get; set; }
        public string Uk { get; set; }
        public string Ru { get; set; }
        public string DescUk { get; set; }
        public string DescRu { get; set; }
        public string Filter { get; set; }
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public virtual Catalog Parent { get; set; }
        public bool IsUsable { get; set; }
        public virtual ICollection<Catalog> Dependencies { get; set; }


        public string UserId { get; set; }
    //    public virtual ApplicationUser User { get; set; }
    }
}

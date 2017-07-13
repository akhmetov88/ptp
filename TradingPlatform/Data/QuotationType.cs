using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("QuotationTypes")]
    public partial class QuotationType : BaseEntity
    {
        //[LDisplayName("displayName","QuotationType","Назва типу котирування","Название типа котировки")]
        public string Type { get; set; }
       // [LDisplayName("displayName", "Quotations", "Список котирувань", "Список котировок")]
        public virtual ICollection<Quotation> Quotations { get; set; }



    }
}

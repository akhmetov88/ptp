using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("Quotations")]
    public partial class Quotation : BaseEntity
    {
        public string Value { get; set; }
        public DateTime Date { get; set; }
        public int QuotationTypeId { get; set; }
        public virtual QuotationType QuotationType { get; set; }
        


    }
}

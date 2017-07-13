using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("News")]
    public partial class New : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public int? TradeId { get; set; }
        public virtual Trade Trade { get; set; }

    }
}

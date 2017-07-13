namespace TradingPlatform.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HelpGroups")]
    public partial class HelpGroup : BaseEntity
    {
        public HelpGroup()
        {
            Helps = new HashSet<Help>();
        }


        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string Hashtag { get; set; }

        public string TitleRu { get; set; }
        public string TitleUa { get; set; }
        public string TitleEn { get; set; }

        public string Ru { get; set; }
        public string Ua { get; set; }
        public string En { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual HashSet<Help> Helps { get; set; }
    }
}

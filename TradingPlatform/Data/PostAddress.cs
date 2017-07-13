using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("PostAddresses")]
    public partial class PostAddress : BaseEntity
    {
        
        public string AbonentBox { get; set; }
        public string ZipCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

        //public int ContragentId { get; set; }
        //public virtual Contragent Contragent { get; set; }
    }
}

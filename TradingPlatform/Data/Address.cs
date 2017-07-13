namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Addresses")]
    public partial class Address : BaseEntity
    {              
        public string ZipCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

    }
}

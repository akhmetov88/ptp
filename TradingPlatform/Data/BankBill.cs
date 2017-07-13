using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("BankBills")]
    public partial class BankBill : BaseEntity
    {
        public string BankName { get; set; }
        public string BankNameEng { get; set; }

        public string Mfo { get; set; }
        public string BillNumber { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsResident { get; set; }

        public string Address { get; set; }
        public string AddressEng { get; set; }

        public bool IsCorrespondent { get; set; }

        public string Iban { get; set; }
        public string Swift { get; set; }

        public int ContragentId { get; set; }
        public virtual Contragent Contragent { get; set; }
    }
}

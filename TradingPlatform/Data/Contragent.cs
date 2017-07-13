using System;
using System.Collections.Generic;
using TradingPlatform.Models;

namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contragents")]
    public partial class Contragent:BaseEntity
    {
        public Contragent()
        {
            BankBills = new List<BankBill>();
            Documents = new List<File>();
            ContragentUsers = new HashSet<ApplicationUser>();
            TradesListBuyer = new HashSet<Trade>();
            TradesListSeller = new HashSet<Trade>();
            Bets = new HashSet<Bet>();
            InBills = new HashSet<TradeBill>();
            OutBills = new HashSet<TradeBill>();
            
        }
   

       // public ICollection<ApplicationUser> ContragentUsersFromidentity { get; set; }

        [StringLength(355)]
        public string LongName { get; set; }
        public string LongNameEng { get; set; }

        public int GroupId { get; set; }
        public virtual Group Group { get; set; }


        [StringLength(10)]
        public string Code { get; set; }
        [StringLength(128)]
        public string TaxCode { get; set; }

        public string ContragentEmail { get; set; }
        public string ContragentSite { get; set; }
        public string ContragentPhone { get; set; }
        public string ContragentFax { get; set; }

        public string ApprovingComment { get; set; }

        public bool IsResident { get; set; }

        public bool IsApproved { get; set; }

        public bool HasContractCopy { get; set; }
        public bool HasContractOriginal { get; set; }
        public bool ContractOnSignin { get; set; }

        public bool IsTaxPayer { get; set; }
        public bool IsDutyPayer { get; set; }
        public string TaxType { get; set; }


        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsConfidant { get; set; }



        [StringLength(128)]
        public string CeoTitle { get; set; }
        [StringLength(128)]
        public string CeoName { get; set; }

        [StringLength(128)]
        public string ConfidantName { get; set; }
        public string ConfidantNameEng { get; set; }

        [StringLength(128)]
        public string ConfidantDocument { get; set; }
        public string ConfidantDocumentEng { get; set; }
        public virtual ICollection<BankBill> BankBills { get; set; } 

        public virtual ICollection<File> Documents { get; set; }
   
        [StringLength(128)]
        public string ApprovedByUserId { get; set; }
        public virtual ApplicationUser ApprovedByUser { get; set; }

        public virtual ICollection<ApplicationUser> ContragentUsers { get; set; }
        
        public string Address { get; set; }
        public string AddressEng { get; set; }
        //public virtual Address Address { get; set; }

        public string PostAddress { get; set; }
        public string PostAddressEng { get; set; }
        // public virtual PostAddress PostAddress { get; set; }

        public virtual ICollection<Trade> TradesListBuyer { get; set; }
        public virtual ICollection<Trade> TradesListSeller { get; set; }
        public virtual ICollection<Bet> Bets { get; set; }

        public virtual ICollection<TradeBill> InBills { get; set; }
        public virtual ICollection<TradeBill> OutBills { get; set; }
        public virtual ICollection<Contract> BuyerContracts { get; set; }
        public virtual ICollection<Contract> SellerContracts { get; set; }
        
    }


}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingPlatform.Data
{
    [Table("TradeBills")]
    public partial class TradeBill : BaseEntity
    {
        public TradeBill()
        {
            
        }
        public TradeBill(Trade trade, Contragent buyer, Contract contract, string addContractName, string billName)
        {
            AddContractName = addContractName;
            BillName = billName;
            Trade = trade;
            Buyer = buyer;
            Seller = trade.Seller;
            Contractt = contract;
            CreateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Название (номер) счета
        /// </summary>
        [StringLength(128)]
        public string BillName { get;set;}
  /// <summary>
  /// Название (номер) дополнительного договора
  /// </summary>
        [StringLength(128)]
        public string AddContractName { get; set; }
        /// <summary>
        /// Айди генерального договора покупатель-продавец
        /// </summary>
        public int ContracttId { get; set; }
        /// <summary>
        /// сам договор
        /// </summary>
        public virtual Contract Contractt { get; set; }
        /// <summary>
        /// Дата создания объекта
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Кому (покупатель)
        /// </summary>
        public int ToContragentId { get; set; }
        public virtual Contragent Buyer { get; set; }
        /// <summary>
        /// От кого (продавец)
        /// </summary>
        public int FromContragentId { get; set; }
        public virtual Contragent Seller { get; set; }
        /// <summary>
        /// Айди торгов
        /// </summary>
        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }

    }
}

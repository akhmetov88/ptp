using System;
using System.Collections.Generic;


namespace TradingPlatform.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contracts")]
    public class Contract :BaseEntity
    {
        public Contract()
        {
            TradeBills = new HashSet<TradeBill>();
        }
        /// <summary>
        /// Конструктор договора
        /// </summary>
        /// <param name="trade"></param>
        /// /// <param name="buyer"></param>
        public Contract(Trade trade, Contragent buyer, string shownname=null)
        {
            Trade = trade;
            //ContractNumber = $"{trade.DateBegin.ToString("yy/MM/dd")}/{Id}-PTP";
            FromContragent = trade.Seller;
            ToContragent = buyer;
            CreateDate = DateTime.UtcNow;
            ShowName = shownname;

            //  TradeBills = new HashSet<TradeBill>();
        }    
        /// <summary>
        /// Должен генерироваться по дате торга в формате YY/MM/DD/nn/ptp-p
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContractNumber
        {
            get
            {
                return ShowName??$"{Trade?.DateBegin.ToString("yyMM-dd")}0{Id}/PTP-T";
            }
            set{}
        }
        public string ShowName { get; set; }
        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }
        
        public virtual ICollection<TradeBill> TradeBills { get; set; }

        public int FromContragentId { get; set; }
        public virtual Contragent FromContragent { get; set; }
        public int ToContragentId { get; set; }
        public virtual Contragent ToContragent { get; set; }
        public DateTime CreateDate { get; set; }

    }

   
}

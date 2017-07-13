using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;

namespace TradingPlatform.Data
{

    [Table("Bets")]
    public partial class Bet :BaseEntity
    {
        public Bet()
        {
            
        }
        public Bet(BetViewModel model, string userId)
        {
            TradeId = model.TradeId;
            BuyerId = model.BuyerId;
            DateBet = DateTime.UtcNow;
            DateUpdate = DateTime.UtcNow;
            IsActual = true;
            IsRedemption = model.IsRedemption;
           // LotsCount = model.LotsCount;
            Price = model.Price;
            Volume = model.Volume;
            AspNetUserId = userId;
            LotId = model.LotId;
            DateBet = DateTime.UtcNow;
            LotsCount = model.LotsCount;
        }

        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }
        public int BuyerId { get; set; }
        public virtual Contragent Buyer { get; set; }
        public decimal? Volume { get; set; }
        /// <summary>
        /// Для обычных торгов - цена ставки, для дифференциалов - размер премии
        /// </summary>
        public decimal? Price { get; set; }
        public int LotsCount { get; set;}
        public bool IsActual { get; set; }
        public bool IsRedemption { get; set; }

        public bool IsRebetted { get; set; }

        public DateTime DateBet { get; set; }
        public DateTime DateUpdate { get; set; }
        [StringLength(128)]
        public string AspNetUserId { get; set; }
        public virtual ApplicationUser AspNetUser { get; set; }

        [StringLength(128)]
        public string RebetterId { get; set; }
        public ApplicationUser Rebetter { get; set; }

        public int? LotId { get; set; }
        public virtual Lot Lot { get; set; }

    }
}

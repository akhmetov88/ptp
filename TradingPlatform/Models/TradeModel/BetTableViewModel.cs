using System;
using TradingPlatform.Data;

namespace TradingPlatform.Models.TradeModel
{
    public class BetTableViewModel
    {
        public int Id { get; set; }
        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }

        public int BuyerId { get; set; }
        public Contragent Buyer { get; set; }

        public decimal? Volume { get; set; }
        public decimal? Price { get; set; }
        public int LotsCount { get; set; }
        public bool IsActual { get; set; }
        public DateTime DateBet { get; set; }
        public DateTime DateUpdate { get; set; }
        
    }
}

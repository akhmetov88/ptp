using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.Data;
using TradingPlatform.Models.EntityModel;

namespace TradingPlatform.Models
{
    public class LotViewModel
    {
        public LotViewModel()
        {

        }
        public LotViewModel(Lot lot, ApplicationUser user=null)
        {
            Id = lot.Id;
            TradeId = lot.TradeId;
            BuyerId = lot.BuyerId??0;
            SellerId = lot.SellerId;
            Volume = lot.Volume;
            Price = lot.Price;
            MinPrice = lot.MinPrice;
            IsActual = lot.IsActual;
            IsSelled = lot.IsSelled;
            IsCurrentUser = lot.Bets.Any(c=>c.IsActual&&user.UserContragents.Select(f=>f.Id).Contains(c.BuyerId));
            Bets = lot.Bets.Select(c => new BetViewModel(c,user)).ToList();
            ElapsingTime = lot.ElapsingTime;
            OnThinking = lot.OnThinking;
            TimeFrom = lot.Trade.DateBegin;
            ReSellingCount = lot.ReSellingCount;
            LotNumber = lot.LotNumber;
        }

        public int Id { get; set; }
        public int TradeId { get; set; }
        public bool IsCurrentUser { get; set; }
        public int? BuyerId { get; set; }

        public int SellerId { get; set; }
   
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal MinPrice { get; set; }
        public bool IsActual { get; set; }
        public bool IsSelled { get; set; }
        public int LotNumber { get; set; }
        public DateTime TimeFrom { get; set; }
        /// <summary>
        /// Время окончания раздумий продавца/покупателя
        /// </summary>
        public DateTime ElapsingTime { get; set; }
        /// <summary>
        /// Идикатор того, что продавец размышляет, что делать с лотом. По настижению времени ElapsingTime происходит switch этого значения
        /// </summary>
        public bool OnThinking { get; set; }

        /// <summary>
        /// Cчётчик перевыставлений
        /// </summary>
        public int ReSellingCount { get; set; }
        public ICollection<BetViewModel> Bets { get; set; }

        

    }
}
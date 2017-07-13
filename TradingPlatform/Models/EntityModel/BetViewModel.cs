using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TradingPlatform.Data;

namespace TradingPlatform.Models.EntityModel
{
    public class BetViewModel
    {
        public BetViewModel()
        { }
        public BetViewModel(Bet bet, ApplicationUser user=null)
        {
            Id = bet.Id;
            TradeId = bet.TradeId;
            LotId =  bet.LotId;
            BuyerName = bet.Buyer.LongName;
            BuyerId = bet.BuyerId;
            Volume = bet.Volume;
            Price =  bet.Price;
            MaxPrice = bet.Trade.MaxPrice;
            Step = bet.Trade.PriceStep;
            LotsCount = bet.Id;
            LotsCountMax = bet.Id;
            LotVolume = bet.Id;
            IsActual = bet.IsActual;
            IsRedemption = bet.IsRedemption;
            IsRebetted = bet.IsRebetted;
            DateBet = bet.DateBet;
            DateUpdate = bet.DateUpdate;
            IsCurrentUserBet = user.UserContragents.Select(c=>c.Id).Contains(bet.BuyerId);
            Currency = StaticData.Inst.GetCatalogValue(bet.Trade.PriceCurrency);
            Unit = StaticData.Inst.GetCatalogValue(bet.Trade.Unit);
            Buyers =
                user?.UserContragents?.Select(c => new SelectListItem() {Text = c.LongName, Value = c.Id.ToString()})?
                    .ToList();
        }

        public BetViewModel(Order order, ApplicationUser user = null)
        {
            IsRedemption = true;
            TradeId = order.TradeId;
            BuyerName = order.Buyer.LongName;
            BuyerId = order.BuyerId;
            Volume = order.Volume;
            Price = order.Price;
            LotsCount = 1;
            LotVolume = order.Volume;
            IsCurrentUserBet = user.UserContragents.Select(c => c.Id).Contains(order.BuyerId);
            Currency = StaticData.Inst.GetCatalogValue(order.Trade.PriceCurrency);
            Unit = StaticData.Inst.GetCatalogValue(order.Trade.Unit);
            IsActual = true;       

        }

        /// <summary>
        /// Конструктор, который принимает объект торгов, список контрагентов текущего юзера, которые могут делать ставки и лот, к которому надо ставку привязать
        /// </summary>
        /// <param name="tradeobject">Торги</param>
        /// <param name="currentusercontragents">Список контрагентов, зарегистрированных на торги</param>
        ///  <param name="lot">Лот, по которому делатеся ставка</param>
        /// <param name="betid">Если генериться модель для перебития, то это айди перебиваемой</param>
        public BetViewModel(Trade tradeobject, IEnumerable<Contragent> currentusercontragents = null, Lot lot=null, int? betid=null)
        {
            LotsCount = 1;
            Buyers = currentusercontragents.Select(c => new SelectListItem() { Text = c.LongName, Value = c.Id.ToString() }).ToList();
            Price = tradeobject.PriceStart==0?lot?.Price: tradeobject.PriceStart;
            TradeId = tradeobject.Id;
            LotsCountMax = tradeobject.LotsCount??0;
            BuyerId = currentusercontragents.FirstOrDefault().Id;
#warning Вроде бы пофиксил отрисовку цены (баг при перевыставлении)
            Price = tradeobject.PriceStart == 0 ? lot?.MinPrice??tradeobject.DifferencialMin : tradeobject.PriceStart;
            MaxPrice = tradeobject.MaxPrice??tradeobject.DifferencialMax;
            BuyerName = currentusercontragents.FirstOrDefault().LongName;
            LotVolume = tradeobject.LotVolume??0;
            Sum = tradeobject.PriceStart*tradeobject.LotVolume??0;
            Step = tradeobject.PriceStep;
            DateEnd = tradeobject.DateEnd;
            LotId = lot?.Id;
            Currency = StaticData.Inst.GetCatalogValue(tradeobject.PriceCurrency);
            Unit = StaticData.Inst.GetCatalogValue(tradeobject.Unit);
            RebetterId = currentusercontragents.FirstOrDefault().Id;
            BetIdToRebet = betid??0;
            RebetterName = currentusercontragents?.FirstOrDefault().LongName;
        }


        public int Id { get; set; }
        public int TradeId { get; set; }
        public bool IsRebetting { get; set; }
        public int? LotId { get; set; }
        public string BuyerName { get; set; }
        public int BuyerId { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Price { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? Step { get; set; }
        public int LotsCount { get; set; }
        public int LotsCountMax { get; set; }
        public decimal LotVolume { get; set; }
        public bool IsActual { get; set; }
        public bool IsRedemption { get; set; }
        public bool IsRebetted { get; set; }
        public DateTime DateBet { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateEnd { get; set; }
        public string RebbetterName { get; set; }
        public bool IsCurrentUserBet { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public int BetIdToRebet { get; set; }
        public int RebetterId { get; set; }
        public string RebetterName { get; set; }

        public decimal Sum
        {
            get
            {
                if (Volume != null) return Price.Value*Volume.Value;
                return Price.Value * LotVolume;
            }
            set { }
        }
        public List<SelectListItem> Buyers { get; set; }
    }
    

}
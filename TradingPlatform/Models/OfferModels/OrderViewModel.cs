using System;
using System.Linq;
using TradingPlatform.Data;

namespace TradingPlatform.Models.OfferModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {

        }
        public OrderViewModel(Order order, ApplicationUser user)
        {
            TradeId = order.TradeId;
            BuyerId = order.BuyerId;
            BuyerName = order.Buyer.LongName;
            IsAcceptedBySeller = order.IsAcceptedBySeller;
            Volume = order.Volume;
            IsCurrentUser = user.UserContragents.Select(c => c.Id).Contains(order.BuyerId);
            Created = order.Created;
            IsForSeller = user.UserContragents.Select(c => c.Id).Contains(order.Trade.SellerId);
            Price = order.Price;
            Id = order.Id;
        }
        public int Id { get; set; }
        public int TradeId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public bool IsAcceptedBySeller { get; set; }
        public int Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Sum => Price * Volume;
        public bool IsCurrentUser { get; set; }
        public bool IsForSeller { get; set; }
        public DateTime Created { get; set; }
    }
}
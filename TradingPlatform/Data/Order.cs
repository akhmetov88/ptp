using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingPlatform.Models;
using TradingPlatform.Models.EntityModel;
using TradingPlatform.Models.OfferModels;
using TradingPlatform.Models.TradeModel;

namespace TradingPlatform.Data
{

    [Table("Orders")]
    public partial class Order : BaseEntity
    {
        public Order()
        {

        }
        public Order(UserOrderViewModel model)
        {
            TradeId = model.OfferId;
            Volume = model.Volume;
            BuyerId = model.SelectedContragent;
            Price = model.Price;
        }

        public int TradeId { get; set; }
        public virtual Trade Trade { get; set; }
        public int Volume { get; set; }
        public bool IsAcceptedBySeller { get; set; }
        public int BuyerId { get; set; }
        public virtual Contragent Buyer { get; set; }
        public decimal Price { get; set; }

    }
}

using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class OrderAccept:BaseNotify
    {
        //public OrderAccept()
        //{

        //}
        public OrderAccept(ApplicationUser user, Order order):base(user)
        {
            OfferNumber = order.TradeId;
            ContragentName = order.Buyer?.LongName??"Contragent";
            Volume = order.Volume;
            Trade = true;
            Subject = "Вашу " + (order.Trade.IsOffer?"пропозицію ":" заявку ") + "прийнято продавцем";
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
        public int Volume;
        public int OfferNumber;
    }
}
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class NewReplyToOffer : BaseNotify
    {
        //public OrderAccept()
        //{

        //}
        public NewReplyToOffer(ApplicationUser user, Order order):base(user)
        {
            OfferNumber = order.TradeId;
            ContragentName = order.Buyer?.LongName??"Contragent";
            Volume = order.Volume;
            Trade = true;
            Subject = "Відгук на " + (order.Trade.IsOffer ? "пропозицію " : " заявку ") + "№ " + OfferNumber;
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
        public int Volume;
        public int OfferNumber;
    }
}
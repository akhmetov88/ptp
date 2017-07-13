using TradingPlatform.Data;
using TradingPlatform.Models.TradeModel;

namespace TradingPlatform.Models.NotifyModels
{
    public class NewOffer : BaseNotify
    {
        /// <summary>
        /// Уведомление о предложении
        /// </summary>
        /// <param name="user"></param>
        /// <param name="trade"></param>
        public NewOffer(ApplicationUser user, TradeViewModel trade) : base(user)
        {
            TradeNumber = trade.Id;
            Model = trade;
            Trade = true;
            Promo = true;
            Link = $"https://ptp.ua/trade/showcurrenttrades?trade=trade-{trade.Id}";
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Нова пропозиція", "Новое предложение", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }
        public TradeViewModel Model;
        public int TradeNumber;
    }
}
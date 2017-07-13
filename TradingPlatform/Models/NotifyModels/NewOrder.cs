using TradingPlatform.Data;
using TradingPlatform.Models.TradeModel;

namespace TradingPlatform.Models.NotifyModels
{
    public class NewOrder : BaseNotify
    {
      //  public NewTrade() { }
      /// <summary>
      /// Уведомление всех о новой заявке на покупку
      /// </summary>
      /// <param name="user"></param>
      /// <param name="trade"></param>
        public NewOrder(ApplicationUser user, TradeViewModel trade) : base(user)
        {
            TradeNumber = trade.Id;
            Model = trade;
            Trade = true;
           // Promo = true;
            Link = $"https://ptp.ua/trade/showcurrenttrades?trade=trade-{trade.Id}";
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Нова заявка", "Новая заявка", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }
        public TradeViewModel Model;
        public int TradeNumber;
    }
}
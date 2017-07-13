using TradingPlatform.Models.TradeModel;

namespace TradingPlatform.Models.NotifyModels
{
    public class NewTrade : BaseNotify
    {
      //  public NewTrade() { }
        public NewTrade(ApplicationUser user, TradeViewModel model) : base(user)
        {
            TradeNumber = model.Id;
            Model = model;
            Trade = true;
            Promo = true;
            Link = $"https://ptp.ua/trade/showcurrenttrades?trade=trade-{model.Id}";
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Нові торги", "Новые торги", user.Locale);
            Send = IsSend(user);
         //   SendMessage();
        }
        public TradeViewModel Model;
        public int TradeNumber;
    }
}
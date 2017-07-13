using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class SellerLetter : BaseNotify
    {
      //  public SellerLetter() { }
        public SellerLetter(ApplicationUser user, Trade trade, int winners) : base(user)
        {
            TradeId = trade.Id;
            Subject = "[Торги № " + TradeId + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Ваші торги успішно завершено", "Ваши торги успешно завершены", user.Locale);
            Important = true;
            Company = trade.Seller.LongName;
            BuyersCount = winners;
            Link = "https://ptp.ua/auction-seller-history";
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
        public int TradeId;
        public int BuyersCount;

    }
}
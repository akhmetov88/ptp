using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;
using TradingPlatform.Enums;

namespace TradingPlatform.Models.NotifyModels
{
    public class CreateTrade : BaseNotify
    {
      //  public CreateTrade() { }
        public  CreateTrade(ApplicationUser user, Trade trade) : base(user)
        {
            TradeNumber = trade.Id;
            TradeDate = trade.DateBegin.ToClientTimeDate().ToString("HH:mm dd.MM.yyyy");
            Link = "https://ptp.ua/trade/showcurrenttrades?trade=trade-" + trade.Id;
            Trade = true;
            Promo = true;
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Оголошено нові торги", "Объявлен новый торг", user.Locale);
            Send = IsSend(user);
            //Snd();
        }
        public int TradeNumber;
        public string TradeDate;
    }
}
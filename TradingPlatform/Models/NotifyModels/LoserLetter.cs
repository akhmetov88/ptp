using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class LoserLetter : BaseNotify
    {
       // public LoserLetter() { }
        public LoserLetter(ApplicationUser user, int tradeId, string company) : base(user)
        {
            Trade = true;
            TradeId = tradeId;
            Company = company;
            Link = "https://ptp.ua/auction-seller-history";
            Subject = "[Торги № " + TradeId + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Торги завершено", "Торги завершены", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
        public int TradeId;
    }
}
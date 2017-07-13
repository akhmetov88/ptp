using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class BetMail : BaseNotify
    {
       // public BetMail() { }
        public BetMail(ApplicationUser user, Bet bet) : base(user)
        {
            Trade = true;
            Link = "https://ptp.ua/auction-buyer-current";
            BetId = bet.Id.ToString();
            TradeNumber = bet.TradeId;
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Ставка перебита", "Ставка перебита", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }

        public int TradeNumber;
        public string BetId;
    }
}
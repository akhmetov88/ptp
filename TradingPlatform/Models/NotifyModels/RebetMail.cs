using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class RebetMail : BaseNotify
    {
       // public RebetMail() { }
        public RebetMail(ApplicationUser user, Bet bettorebet, Bet bet) : base(user)
        {
            TradeNumber = bet.TradeId;
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Ставка перебита", "Ставка перебита", user.Locale);
            Trade = true;
            Link = "https://ptp.ua/auction-buyer-current";
            BetId = bettorebet.Id + "/" + bettorebet.DateBet;
            RebetterName = bet.Buyer.LongName;
            Send = IsSend(user);
            //SendMessage();
        }
        public int TradeNumber;
        public string BetId;
        public string RebetterName;
    }
}
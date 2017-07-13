using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;
using TradingPlatform.Messaging;

namespace TradingPlatform.Models.NotifyModels
{
    public class Redemption:BaseNotify
    {
        //public Redemption()
        //{

        //}
        public Redemption(ApplicationUser user, Bet bet):base(user)
        {
            Trade = true;
            ContragentName = bet.Buyer.LongName;
            LotsCount = bet.LotsCount;
            TradeNumber = bet.TradeId;
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Лоти викуплено", "Лоты выкуплены", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
        public int LotsCount;
        public int TradeNumber;
    }
}
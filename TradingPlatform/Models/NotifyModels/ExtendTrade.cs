using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class ExtendTrade : BaseNotify
    {
       // public ExtendTrade() { }
        public ExtendTrade(ApplicationUser user, int id, int minutes) : base(user)
        {
            Trade = true;
            TradeNumber = id;
            Min = minutes.ToString();
            Subject = "[Торги № " + TradeNumber + "] " + LocalText.Inst.Get("emailSubject", this.ToString(), "Продовження торгів", "Продление торгов", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }
        public int TradeNumber;
        public string Min;
    }
}
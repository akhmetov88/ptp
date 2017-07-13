using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class ApproveTrade : BaseNotify
    {
       // public ApproveTrade() { }
        public ApproveTrade(ApplicationUser user, string contragent) : base(user)
        {
            Important = true;
            ContragentName = contragent;
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Торги доступні для покупців", "Торг доступен для покупателей", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
    }
}
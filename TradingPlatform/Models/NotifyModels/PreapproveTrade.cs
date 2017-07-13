using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class PreapproveTrade : BaseNotify
    {
     //   public PreapproveTrade() { }

        public PreapproveTrade (ApplicationUser user,string contragent) : base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Торги схвалено", "Торг одобрен", user.Locale);
            Important = true;
            ContragentName = contragent;
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
    }
}
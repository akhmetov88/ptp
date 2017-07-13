using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class DeactiveMail:BaseNotify
    {
        //public DeactiveMail() { }

        public DeactiveMail(ApplicationUser user, string contragent):base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Акредитацію знято", "Снята аккредитация", user.Locale);
            Important = true;
            ContragentName = contragent;
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
    }
}
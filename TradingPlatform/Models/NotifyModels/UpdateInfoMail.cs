using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class UpdateInfoMail:BaseNotify
    {
      //  public UpdateInfoMail() { }

        public UpdateInfoMail(ApplicationUser user, string contragent, string link):base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Інформацію про контрагента оновлено", "Обновлена информация о контрагенте","uk-UA");
            Important = true;
            ContragentName = contragent;
            Link = link;
            UserName = "Працівник юридичного відділу";
            Email = "info@ptp.ua";
            Send = IsSend(user);
            //SendMessage();
        }

        public string ContragentName;
    }
}
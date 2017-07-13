using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class ToLawyerAboutNewCompany : BaseNotify
    {
      //  public ToLawyerAboutNewCompany() { }
        public ToLawyerAboutNewCompany(ApplicationUser user, string contragent, string link) : base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Новий контрагент", "Новый контрагент", user.Locale);
            Important = true;
            UserName = "Працівник юридичного відділу";
            Email = "info@ptp.ua";
            Company = contragent;
            Link = link;
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
    }
}
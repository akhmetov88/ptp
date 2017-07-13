using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradingPlatform.Models.NotifyModels
{
    public class ToUserAboutNewCompany : BaseNotify
    {
      //  public ToLawyerAboutNewCompany() { }
        public ToUserAboutNewCompany(ApplicationUser user, string contragent, string link) : base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Успішна заявка на реєстрацію", "Успешная заявка на регистрацию", user.Locale);
            Important = true;
            //UserName = user.RegisterName;
            //Email = user.Email;
            Company = contragent;
            Link = link;
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
    }
}
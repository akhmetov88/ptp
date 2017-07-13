using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class LegalNotActive : BaseNotify
    {
       // public LegalNotActive() { }

        public LegalNotActive(ApplicationUser user, Contragent contragent) : base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Помилка при реєстрації юридичної особи", "Ошибка при регистрации юридического лица", user.Locale);
            Important = true;
            Company = contragent.LongName;
            Comment = contragent.ApprovingComment;
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
        public string Docs;
        public string Comment;
    }
}
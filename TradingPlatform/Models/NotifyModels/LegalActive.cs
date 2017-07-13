using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class LegalActive : BaseNotify
    {
       // public LegalActive() { }

        public LegalActive(ApplicationUser user, Contragent contragent) : base(user)
        {
            Subject = LocalText.Inst.Get("emailSubject ", this.ToString(), "Успiшна реєстрація юрдичної особи", "Успешная регистрация юридического лица");
            Important = true;
            Role = isBuyerSeller(contragent.IsBuyer, contragent.IsSeller);
            Company = contragent.LongName;
            Send = IsSend(user);
            //SendMessage();
        }

        public string Company;
        public string Role;

        private string isBuyerSeller(bool isBuyer, bool isSeller)
        {
            if (isBuyer && isSeller)
                return String.Format("{0} {1} {2}",
                    LocalText.Inst.Get("code", "mail - buyer", "покупець", "покупатель"),
                    LocalText.Inst.Get("code", "mail-and", "та", "и"),
                    LocalText.Inst.Get("code", "mail-seller", "продавець", "продавец"));

            if (isBuyer)
                return LocalText.Inst.Get("code", "mail - buyer", "покупець", "покупатель");

            if (isSeller)
                return LocalText.Inst.Get("code", "mail-seller", "продавець", "продавец");

            return string.Empty;
        }
    }
}
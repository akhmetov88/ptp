
using TradingPlatform.Messaging;

namespace TradingPlatform.Models.NotifyModels
{
    public class SuccessRegister : BaseNotify
    {
     //   public SuccessRegister() { }
        public SuccessRegister(ApplicationUser user, string link) : base(user)
        {
           // Message = EmailFactory.RenderViewToString(this.ToString(), this);
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Дякуємо за реєстрацію на РТР.UA!", "Спасибо за регистрацию на PTP.UA!", user.Locale);
            Important = true;
            Link = link;
            Send = IsSend(user);
            //SendMessage();
        }

       
    }
}
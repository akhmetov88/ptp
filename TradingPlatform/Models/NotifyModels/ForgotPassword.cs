
using TradingPlatform.Messaging;

namespace TradingPlatform.Models.NotifyModels
{
    public class ForgotPassword : BaseNotify
    {
     //   public ForgotPassword() { }
        public ForgotPassword(ApplicationUser user, string link) : base(user)
        {
            Important = true;
            Link = link;
            Subject = LocalText.Inst.Get("emailSubject", this.ToString(), "Зміна паролю", "Смена пароля", user.Locale);
            Send = IsSend(user);
            //SendMessage();
        }
    }
}
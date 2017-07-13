using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TradingPlatform.Messaging;
using TradingPlatform.Models.ViewModel;


namespace TradingPlatform.Models.NotifyModels
{
    public abstract class BaseNotify
    {
        //protected BaseNotify()        {       }

        protected BaseNotify(ApplicationUser user/*, params dynamic[] par*/)
        {
            UserName = user.RegisterName;
            Lang = user.Locale;
            Email = user.Email;

            //Send = user.IsDebug || Important;//||user.AllowPromoEmails!=Promo||user.AllowTradeEmails!=Trade;
            SignalRAddress = user.UserName;          
          
        }
        public string SignalRAddress;
        public string Address;
        public string Email;
        public string UserName;
        public string Link;
        public string Lang;
        public string Message;
        public string Subject;
        public bool Send;
        public bool Broadcast;
        public bool Important;
        public bool Promo;
        public bool Trade;

        public override string ToString()
        {
            return this.GetType().Name;
        }

        //public virtual void SendMessage()
        //{
        //    if (Send)
        //    {
        //        EmailFactory.SendEmail(new Email(this));
        //    }
        //    SendSignalr(this);
        //}


        public virtual bool IsSend(ApplicationUser user)
        {
#if DEBUG
            return user.IsDebug&&Email!="info@ptp.ua";// || user.AllowPromoEmails&& Promo || user.AllowTradeEmails&&Trade||Important;
#else
            return user.AllowPromoEmails&&this.Promo||user.AllowTradeEmails&&Trade||Important;
#endif
        }

        //public virtual async Task SendMessageAsync()
        //{
        //    if (Send)
        //    {
        //      await EmailFactory.SendEmailAsync(new Email(this));
        //    }
        //   await  SendSignalrAsync(this);
        //}
        
        private async Task SendSignalrAsync(BaseNotify notify)
        {
            var message = new NotificationViewModel(notify);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            await context.Clients.User(message.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(message)));
        }
        private  void SendSignalr(BaseNotify notify)
        {
            var message = new NotificationViewModel(notify);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(message.ToUserName).renderNotification(JObject.Parse(JsonConvert.SerializeObject(message)));
        }
    }
}

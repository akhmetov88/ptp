
using TradingPlatform.Messaging;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Models
{
    public class Email
    {
        public Email(BaseNotify notify)
        {
            ToEmail = notify.Email;
            Message = EmailFactory.RenderViewToString(notify.ToString(), notify);
            Subject = notify.Subject;
            //  Send = notify.Send;
        }
        public Email(Broadcast notify)
        {
            ToEmail = notify.Email;
            Message = EmailFactory.RenderViewToString(notify.ToString(), notify);
            Subject = notify.Subject;
            //  Send = notify.Send;
        }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        //public bool Send { get; set; }
    }
}
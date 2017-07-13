using System;
using TradingPlatform.Models.NotifyModels;

namespace TradingPlatform.Models.ViewModel
{
    public class NotificationViewModel
    {
        public NotificationViewModel()
        {
       
        }
        public NotificationViewModel(BaseNotify notify)
        {
            Subject = notify.Subject;
            Body = LocalText.Inst.Get("signalRBodyText", notify.ToString(), "", "", notify.Lang);
            RedirectUrl = notify.Link;
            ToUserName = notify.SignalRAddress;
        }
        public int Id { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public bool IsViewed { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ViewedDate { get; set; }

        public string FromUserId { get; set; }
        public string FromUserName { get; set; }

        public string RedirectUrl { get; set; }
        
        public string ToUserId { get; set; }
        public string ToUserName { get; set; }
    }
}
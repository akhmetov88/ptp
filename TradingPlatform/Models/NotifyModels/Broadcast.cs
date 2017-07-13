using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class Broadcast 
    {
        public Broadcast()
        {
            IsReply = false;
            ToAll = false;
        }
        public Broadcast(Feedback model)
        {
            Subject = "RE: " + model.Subject;
            Email = model.Email;
            UserName = model.Name;
            Body = model.Text;
            IsReply = true;
            ReplyId = model.Id;
            ToAll = false;
        }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Body { get; set; }
        public bool ToAll { get; set; }
        public bool ToTraders { get; set; }
        public bool ToBuyers { get; set; }
        public  bool IsReply { get; set; }
        public int ReplyId { get; set; }
        public  string txtContentHtmlArea { get; set; }
        public override string ToString()
        {
            return "Broadcast";
        }
    }
}
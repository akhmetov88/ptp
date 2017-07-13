using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsModule.Abstractions;

namespace NotificationsModule.MailSender
{
    public static class Sender
    {
        static MailSender _sender = new MailSender();

        public static void Send(IMessage m)
        {
            _sender.SendMessage(m);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using NotificationsModule.Abstractions;

namespace NotificationsModule.MailSender
{
    public class MailSender : IMessageSender
    {
        private static readonly string _Alias = "mail@ptp.ua";
        private static readonly string _Domain = "kyiv.onmicrosoft.com";
        private static readonly string _Password = "V'h,Z!3vS&*7(y,g";
        private static readonly string _UserName = "ptpinfo@kyiv.onmicrosoft.com";
        private static readonly string _smtp = "smtp.office365.com";
        private static readonly int _port = 587;

        private static  SmtpClient _smtpClient = new MyClass();

        private class MyClass : SmtpClient
        {
            public MyClass()
            {
                this.Connect(_smtp, _port);
                this.Authenticate(new NetworkCredential()
                {
                    Domain = _Domain,
                    Password = _Password,
                    UserName = _UserName
                });
            }
        }
        public void SendMessage(IMessage m)
        {
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress("Joey", _Alias));
            message.To.Add(new MailboxAddress("Alice", m.MessageReceiver.Address));
            message.Subject = m.Subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = m.Body??""
            };
            
            Send(message,_smtpClient);
        }

        public void SendMessages(ICollection<IMessage> message)
        {
            throw new NotImplementedException();
        }

        private  void Send(MimeMessage message, SmtpClient client)
        {
           
            if (client.IsAuthenticated&&client.IsConnected)
            {
                client.Send(message);
                client.Disconnect(true);
            }
            else
            {
                client.Connect(_smtp, _port);
                client.Authenticate(new NetworkCredential()
                {
                    Domain = _Domain,
                    Password = _Password,
                    UserName = _UserName
                });
            }


        }

        void IMessageSender.SendMessage(IMessage message)
        {
            SendMessage(message);
        }
    }
}

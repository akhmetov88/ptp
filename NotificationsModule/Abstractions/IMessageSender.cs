using System.Collections.Generic;

namespace NotificationsModule.Abstractions
{
    public interface IMessageSender
    {
        void SendMessage(IMessage message);
  
        void SendMessages(ICollection<IMessage> message);

    }
}
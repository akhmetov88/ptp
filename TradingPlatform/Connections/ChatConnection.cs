using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using TradingPlatform.Models;

namespace TradingPlatform.Connections
{
    public class ChatConnection : PersistentConnection
    {

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            IConnectionGroupManager groupManager = new GroupManager(Connection,"newGroup");

            SignalMessage chatData = new SignalMessage() { Name = "Сообщение сервера", Message = "Пользователь " + connectionId + " присоединился к чату" };
            return Connection.Broadcast(chatData);
        }
       
        
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            SignalMessage chatData = JsonConvert.DeserializeObject<SignalMessage>(data);

            return Connection.Broadcast(chatData);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            SignalMessage chatData = new SignalMessage() { Name = "Сообщение сервера", Message = "Пользователь " + connectionId + " покинул чат" };
            return Connection.Broadcast(chatData);
        }
    }
}
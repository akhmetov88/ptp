namespace NotificationsModule.Abstractions
{
    public interface IMessageCreator
    {
        IMessage CreateMessage(string messagetype, string userId);
        IMessage CreateMessage(params object[] p);
    }
}
namespace NotificationsModule.Abstractions
{
    public interface IMessage
    {
        IReceiver MessageReceiver { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
    }
}
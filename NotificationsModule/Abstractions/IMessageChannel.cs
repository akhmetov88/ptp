namespace NotificationsModule.Abstractions
{
    public interface IMessageChannel
    {
        string ChannelName { get; set; } 
        object ChannelCredentials { get; set; }
    }
}
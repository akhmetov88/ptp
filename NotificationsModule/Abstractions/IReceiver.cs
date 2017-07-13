namespace NotificationsModule.Abstractions
{
    public interface IReceiver
    {
        string Address { get; set; }
        string Name { get; set; }
    }
}
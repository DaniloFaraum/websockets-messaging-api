namespace WebKafka.Messaging.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync(string topic, string message);
        void Subscribe(string topic, Action<string> handler);
    }
}

namespace Implementation.EventBridge
{
    public class OrderStatusChangedEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class EventPublisher : Interfaces.IEventPublisher
    {
        public Task PublishAsync<T>(T eventData) where T : class
        {
            // Здесь отправка события в AWS EventBridge
            return Task.CompletedTask;
        }
    }
} 
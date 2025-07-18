namespace Implementation
{
    public class OrderStatusChangedEvent
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
    }

    public static class EventBridge
    {
        public static Task PublishAsync(OrderStatusChangedEvent evt)
        {
            // Здесь отправка события в AWS EventBridge
            return Task.CompletedTask;
        }
    }
} 
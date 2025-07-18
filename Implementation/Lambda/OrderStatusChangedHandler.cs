using System.Threading.Tasks;

namespace Implementation.EventHandlers
{
    public class OrderStatusChangedEvent
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
    }

    public static class EventHandlerLambda
    {
        public static async Task Handle(OrderStatusChangedEvent evt)
        {
            if (evt.Status == "confirmed")
            {
                // TODO: создать заказ в доставке
            }
            else if (evt.Status == "payment_failed")
            {
                await NotificationService.NotifyPaymentFailed(evt.OrderId);
            }
        }
    }

    // Заглушка NotificationService
    public static class NotificationService
    {
        public static Task NotifyPaymentFailed(string orderId)
        {
            // Отправить уведомление пользователю
            return Task.CompletedTask;
        }
    }
} 
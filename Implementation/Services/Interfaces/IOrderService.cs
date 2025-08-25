namespace Implementation.Services.Interfaces;

using Implementation.Models;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(string userId, string cartId, Address shippingAddress);
    Task<List<Order>> GetExpiredOrdersPaginatedAsync(DateTime cutoffTime, int limit, int offset);
    Task DeleteOrdersBulkAsync(List<string> orderIds);
    Task UpdateOrderStatusAndPublishAsync(string orderId, string status);
}

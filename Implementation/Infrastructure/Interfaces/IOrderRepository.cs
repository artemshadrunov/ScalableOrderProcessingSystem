namespace Implementation.Infrastructure.Interfaces;

using Implementation.Models;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task<List<Order>> GetExpiredOrdersPaginatedAsync(DateTime cutoffTime, int limit, int offset);
    Task DeleteOrdersBulkAsync(List<string> orderIds);
    Task<Order?> GetByIdAsync(string orderId);
    Task UpdateAsync(Order order);
}

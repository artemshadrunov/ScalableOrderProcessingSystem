namespace Implementation.Infrastructure;

using Implementation.Models;
using Implementation.Infrastructure.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _dbContext;
    
    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetExpiredOrdersPaginatedAsync(DateTime cutoffTime, int limit, int offset)
    {
        if (limit <= 0)
            throw new ArgumentException("Limit must be greater than zero", nameof(limit));
        
        if (offset < 0)
            throw new ArgumentException("Offset must be non-negative", nameof(offset));

        return await _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.ExpiresAt <= cutoffTime && o.Status == "cancelled")
            .OrderBy(o => o.OrderId) // Стабильная сортировка для консистентной пагинации
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Массовое удаление заказов. Raw SQL для эффективных массовых операций.
    /// </summary>
    public async Task DeleteOrdersBulkAsync(List<string> orderIds)
    {
        if (orderIds == null)
            throw new ArgumentNullException(nameof(orderIds));
        
        if (!orderIds.Any()) 
            return;
        
        // Проверяем, что заказы все еще существуют и просрочены
        var validOrderIds = await _dbContext.Orders
            .Where(o => orderIds.Contains(o.OrderId) && 
                       o.ExpiresAt <= DateTime.UtcNow && 
                       o.Status == "pending")
            .Select(o => o.OrderId)
            .ToListAsync();
        
        if (!validOrderIds.Any()) 
            return;
        
        // Используем raw SQL для эффективного массового удаления
        var orderIdsString = string.Join(",", validOrderIds.Select(id => $"'{id}'"));
        await _dbContext.Database.ExecuteSqlRawAsync(
            $"DELETE FROM orders WHERE order_id IN ({orderIdsString})");
    }

    public async Task<Order?> GetByIdAsync(string orderId)
    {
        if (string.IsNullOrEmpty(orderId))
            throw new ArgumentException("Order ID cannot be null or empty", nameof(orderId));

        return await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task UpdateAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
} 
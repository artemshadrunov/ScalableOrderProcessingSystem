namespace Implementation.Infrastructure;

using Implementation.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class OrderRepository
{
    private readonly OrdersDbContext _dbContext;
    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Order CreateOrder(Order order)
    {
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();
        return order;
    }

    // Метод для пагинации при обработке больших объемов
    public async Task<List<Order>> GetExpiredOrdersPaginated(DateTime cutoffTime, int limit, int offset)
    {
        return await _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.ExpiresAt <= cutoffTime && o.Status == "cancelled")
            .OrderBy(o => o.OrderId) // Стабильная сортировка для пагинации
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task DeleteOrdersBulk(List<string> orderIds)
    {
        if (!orderIds.Any()) return;
        
        // Проверяем что заказы все еще существуют и просрочены
        var validOrderIds = await _dbContext.Orders
            .Where(o => orderIds.Contains(o.OrderId) && o.ExpiresAt <= DateTime.UtcNow && o.Status == "pending")
            .Select(o => o.OrderId)
            .ToListAsync();
        
        if (!validOrderIds.Any()) return;
        
        // Удаляем заказы пачкой
        var orderIdsString = string.Join(",", validOrderIds.Select(id => $"'{id}'"));
        
        await _dbContext.Database.ExecuteSqlRawAsync(
            $"DELETE FROM orders WHERE order_id IN ({orderIdsString})");
    }

    public async Task<Order?> GetByIdAsync(string orderId)
    {
        return await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
} 
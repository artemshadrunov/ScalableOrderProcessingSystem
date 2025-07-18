namespace Implementation.Infrastructure;

using Implementation.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class StockRepository
{
    private readonly OrdersDbContext _dbContext;
    public StockRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void ReserveStockWithLock(List<CartItem> items)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("Список товаров не может быть пустым");

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Sku) || item.Qty <= 0)
                throw new ArgumentException($"Некорректные данные товара: SKU={item.Sku}, Qty={item.Qty}");

            // Блокируем строку для предотвращения гонки (FOR UPDATE)
            var stock = _dbContext.Stocks
                .FromSqlRaw("SELECT * FROM stock WHERE sku = {0} FOR UPDATE", item.Sku)
                .FirstOrDefault();
            
            if (stock == null)
                throw new Exception($"Товар с SKU {item.Sku} не найден на складе");
            
            var availableQuantity = stock.Quantity - stock.Reserved;
            if (availableQuantity < item.Qty)
                throw new Exception($"Недостаточно товара для SKU: {item.Sku}. Доступно: {availableQuantity}, запрошено: {item.Qty}");
            
            // Резервируем товар
            stock.Reserved += item.Qty;
        }
    }

    public async Task ChangeStocks(Dictionary<string, int> stockToRelease)
    {
        foreach (var kvp in stockToRelease)
        {
            var sku = kvp.Key;
            var quantityToRelease = kvp.Value;
            
            // Блокируем строку для атомарного обновления
            var stock = await _dbContext.Stocks
                .FromSqlRaw("SELECT * FROM stock WHERE sku = {0} FOR UPDATE", sku)
                .FirstOrDefaultAsync();
            
            if (stock != null)
            {
                stock.Reserved = Math.Max(0, stock.Reserved - quantityToRelease);
            }
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Stock>> GetStocksBySkusAsync(IEnumerable<string> skus)
    {
        return await _dbContext.Stocks.Where(s => skus.Contains(s.Sku)).ToListAsync();
    }
} 
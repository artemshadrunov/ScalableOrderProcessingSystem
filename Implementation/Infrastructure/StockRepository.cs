namespace Implementation.Infrastructure;

using Implementation.Models;
using Implementation.Infrastructure.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Реализация репозитория для работы со складскими остатками.
/// Демонстрирует использование блокировок для предотвращения race conditions
/// и атомарные операции с использованием FOR UPDATE.
/// </summary>
public class StockRepository : IStockRepository
{
    private readonly OrdersDbContext _dbContext;
    
    public StockRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Резервирует товары с использованием блокировок для предотвращения race conditions.
    /// Демонстрирует использование FOR UPDATE для атомарных операций.
    /// </summary>
    public async Task ReserveStockWithLockAsync(List<CartItem> items)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("Список товаров не может быть пустым", nameof(items));

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Sku) || item.Qty <= 0)
                throw new ArgumentException($"Некорректные данные товара: SKU={item.Sku}, Qty={item.Qty}", nameof(items));

            // Используем FOR UPDATE для блокировки строки и предотвращения race conditions
            var stock = await _dbContext.Stocks
                .FromSqlRaw("SELECT * FROM stock WHERE sku = {0} FOR UPDATE", item.Sku)
                .FirstOrDefaultAsync();
            
            if (stock == null)
                throw new InvalidOperationException($"Товар с SKU {item.Sku} не найден на складе");
            
            var availableQuantity = stock.Quantity - stock.Reserved;
            if (availableQuantity < item.Qty)
                throw new InvalidOperationException($"Недостаточно товара для SKU: {item.Sku}. Доступно: {availableQuantity}, запрошено: {item.Qty}");
            
            stock.Reserved += item.Qty;
        }
    }

    public async Task ChangeStocksAsync(Dictionary<string, int> stockToRelease)
    {
        if (stockToRelease == null)
            throw new ArgumentNullException(nameof(stockToRelease));

        foreach (var (sku, quantityToRelease) in stockToRelease)
        {
            if (string.IsNullOrEmpty(sku))
                continue;
            
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
        if (skus == null)
            throw new ArgumentNullException(nameof(skus));

        var skuList = skus.ToList();
        if (!skuList.Any())
            return new List<Stock>();

        return await _dbContext.Stocks
            .Where(s => skuList.Contains(s.Sku))
            .ToListAsync();
    }
} 
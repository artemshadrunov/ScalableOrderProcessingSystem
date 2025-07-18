namespace Implementation.Services;

using System.Collections.Generic;
using Implementation.Infrastructure;
using Implementation.Models;
using System.Threading.Tasks;
using System.Linq;

public class StockService
{
    private readonly StockRepository _stockRepository;
    
    public StockService(StockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task ReleaseStockBulk(Dictionary<string, int> stockToRelease)
    {
        await _stockRepository.ChangeStocks(stockToRelease);
    }

    public async Task<bool> CheckAvailability(List<CartItem> items)
    {
        var skus = items.Select(i => i.Sku).ToList();
        var stocks = await _stockRepository.GetStocksBySkusAsync(skus);

        var stockDict = stocks.ToDictionary(s => s.Sku, s => s);
        foreach (var item in items)
        {
            if (!stockDict.TryGetValue(item.Sku, out var stock) || stock.Quantity - stock.Reserved < item.Qty)
                return false;
        }

        return true;
    }
} 
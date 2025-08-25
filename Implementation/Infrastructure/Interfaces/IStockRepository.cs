namespace Implementation.Infrastructure.Interfaces;

using Implementation.Models;

public interface IStockRepository
{
    Task ReserveStockWithLockAsync(List<CartItem> items);
    Task ChangeStocksAsync(Dictionary<string, int> stockToRelease);
    Task<List<Stock>> GetStocksBySkusAsync(IEnumerable<string> skus);
}

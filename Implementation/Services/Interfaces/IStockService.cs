namespace Implementation.Services.Interfaces;

using Implementation.Models;

public interface IStockService
{
    Task<bool> CheckAvailability(List<CartItem> items);
}

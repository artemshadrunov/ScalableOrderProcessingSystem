namespace Implementation.Core;

using System.Linq;
using Implementation.Infrastructure;
using Implementation.Models;

public class CartService
{
    private readonly OrdersDbContext _dbContext;
    public CartService(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Cart? GetCartById(string cartId)
    {
        return _dbContext.Carts.FirstOrDefault(c => c.CartId == cartId);
    }
} 
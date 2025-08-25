namespace Implementation.Services;

using System.Linq;
using Implementation.Infrastructure;
using Implementation.Models;
using Implementation.Services.Interfaces;

public class CartService : ICartService
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
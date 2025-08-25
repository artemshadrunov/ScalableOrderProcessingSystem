namespace Implementation.Services.Interfaces;

using Implementation.Models;

public interface ICartService
{
    Cart? GetCartById(string cartId);
}

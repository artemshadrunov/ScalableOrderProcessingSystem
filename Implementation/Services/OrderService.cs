namespace Implementation.Core;

using System;
using Implementation.Infrastructure;
using Implementation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderService
{
    private readonly CartService _cartService;
    private readonly OrderRepository _orderRepository;
    private readonly StockRepository _stockRepository;
    private readonly OrdersDbContext _dbContext;
    private readonly StockService _stockService;

    public OrderService(CartService cartService, OrderRepository orderRepository, 
                      StockRepository stockRepository, OrdersDbContext dbContext, StockService stockService)
    {
        _cartService = cartService;
        _orderRepository = orderRepository;
        _stockRepository = stockRepository;
        _dbContext = dbContext;
        _stockService = stockService;
    }

    public async Task<Order> CreateOrderAsync(string userId, string cartId, Address shippingAddress)
    {
        var cart = _cartService.GetCartById(cartId);
        if (cart == null)
            throw new Exception("Корзина не найдена");

        var allAvailable = await _stockService.CheckAvailability(cart.Items);
        if (!allAvailable)
            // TODO: Вернуть детальный список недоступных товаров
            throw new Exception("Один или несколько товаров из корзины недоступны на складе в нужном количестве");

        var order = new Order
        {
            OrderId = Guid.NewGuid().ToString(),
            UserId = userId,
            CartId = cartId,
            Items = cart.Items,
            Status = "pending",
            TotalCents = cart.Items.Sum(i => i.PriceCents * i.Qty),
            Address = $"{shippingAddress.Street}, {shippingAddress.City}, {shippingAddress.PostalCode}, {shippingAddress.Country}",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        // Атомарная операция: резервирование stock + создание заказа
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            // Резервируем товары с блокировкой строк
            _stockRepository.ReserveStockWithLock(cart.Items);
            
            // Создаем заказ
            _orderRepository.CreateOrder(order);
            
            // Фиксируем транзакцию
            transaction.Commit();
            
            return order;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<Order>> GetExpiredOrdersPaginated(DateTime cutoffTime, int limit, int offset)
    {
        return await _orderRepository.GetExpiredOrdersPaginated(cutoffTime, limit, offset);
    }

    public async Task DeleteOrdersBulk(List<string> orderIds)
    {
        await _orderRepository.DeleteOrdersBulk(orderIds);
    }
} 
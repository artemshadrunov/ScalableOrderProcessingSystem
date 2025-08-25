namespace Implementation.Services;

using System;
using Implementation.Infrastructure;
using Implementation.Infrastructure.Interfaces;
using Implementation.Models;
using Implementation.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Сервис для управления заказами. Cоздание заказов с использованием транзакций
/// и интеграция с другими сервисами (корзина, склад).
/// </summary>
public class OrderService : IOrderService
{
    private readonly ICartService _cartService;
    private readonly IOrderRepository _orderRepository;
    private readonly IStockRepository _stockRepository;
    private readonly OrdersDbContext _dbContext;
    private readonly IStockService _stockService;

    public OrderService(ICartService cartService, IOrderRepository orderRepository, 
                      IStockRepository stockRepository, OrdersDbContext dbContext, IStockService stockService)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
    }

    /// <summary>
    /// Создает новый заказ с атомарной операцией резервирования товаров.
    /// Использование транзакций для обеспечения консистентности данных.
    /// </summary>
    public async Task<Order> CreateOrderAsync(string userId, string cartId, Address shippingAddress)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        
        if (string.IsNullOrEmpty(cartId))
            throw new ArgumentException("Cart ID cannot be null or empty", nameof(cartId));
        
        if (shippingAddress == null)
            throw new ArgumentNullException(nameof(shippingAddress));

        var cart = _cartService.GetCartById(cartId);
        if (cart == null)
            throw new InvalidOperationException($"Cart with ID '{cartId}' not found");

        var allAvailable = await _stockService.CheckAvailability(cart.Items);
        if (!allAvailable)
            // TODO: Вернуть детальный список недоступных товаров
            throw new InvalidOperationException("One or more items in the cart are not available in the required quantity");

        var order = new Order
        {
            OrderId = Guid.NewGuid().ToString(),
            UserId = userId,
            CartId = cartId,
            Items = cart.Items,
            Status = "pending",
            TotalCents = cart.Items.Sum(i => i.PriceCents * i.Qty),
            Address = FormatShippingAddress(shippingAddress),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        // Атомарная операция: резервирование товаров + создание заказа
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            await _stockRepository.ReserveStockWithLockAsync(cart.Items);
            await _orderRepository.CreateOrderAsync(order);
            transaction.Commit();
            return order;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<Order>> GetExpiredOrdersPaginatedAsync(DateTime cutoffTime, int limit, int offset)
    {
        return await _orderRepository.GetExpiredOrdersPaginatedAsync(cutoffTime, limit, offset);
    }

    public async Task DeleteOrdersBulkAsync(List<string> orderIds)
    {
        await _orderRepository.DeleteOrdersBulkAsync(orderIds);
    }

    public async Task UpdateOrderStatusAndPublishAsync(string orderId, string status)
    {
        if (string.IsNullOrEmpty(orderId))
            throw new ArgumentException("Order ID cannot be null or empty", nameof(orderId));
        
        if (string.IsNullOrEmpty(status))
            throw new ArgumentException("Status cannot be null or empty", nameof(status));

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) 
            throw new InvalidOperationException($"Order with ID '{orderId}' not found");
            
        order.Status = status;
        await _orderRepository.UpdateAsync(order);

        // TODO: Отправка события в EventBridge для уведомления других сервисов
        // await _eventPublisher.PublishAsync(new OrderStatusChangedEvent { ... });
    }

    private static string FormatShippingAddress(Address address)
    {
        return $"{address.Street}, {address.City}, {address.PostalCode}, {address.Country}";
    }
} 
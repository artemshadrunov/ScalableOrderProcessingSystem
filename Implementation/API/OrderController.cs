namespace Implementation.API;

using Microsoft.AspNetCore.Mvc;
using Implementation.Services;
using Implementation.Models;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // POST api/orders
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = _orderService.CreateOrder(request.UserId, request.CartId, request.ShippingAddress, request.PaymentMethod);
        return Ok(order);
    }
}

public class CreateOrderRequest
{
    public string UserId { get; set; }
    public string CartId { get; set; }
    public Address ShippingAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
} 
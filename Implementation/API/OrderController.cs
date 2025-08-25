namespace Implementation.API;

using Microsoft.AspNetCore.Mvc;
using Implementation.Services.Interfaces;
using Implementation.Models;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var address = new Address
            {
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                PostalCode = request.ShippingAddress.PostalCode,
                Country = request.ShippingAddress.Country
            };

            var order = await _orderService.CreateOrderAsync(request.UserId, request.CartId, address);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class CreateOrderRequest
{
    public string UserId { get; set; } = string.Empty;
    public string CartId { get; set; } = string.Empty;
    public AddressDto ShippingAddress { get; set; } = new();
    public PaymentMethodDto PaymentMethod { get; set; } = new();
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class PaymentMethodDto
{
    public string Type { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
} 
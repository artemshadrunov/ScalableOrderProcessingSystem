using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Implementation.Services;
using Implementation.Models;

namespace Implementation.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest req)
        {
            var payment = await _paymentService.CreatePaymentAsync(req.OrderId);
            return CreatedAtAction(nameof(InitiatePayment), new { id = payment.PaymentId }, new { payment_id = payment.PaymentId });
        }

        [HttpPost("webhook/succeeded")]
        public async Task<IActionResult> PaymentSucceeded([FromBody] PaymentWebhookRequest req)
        {
            await _paymentService.MarkPaymentCapturedAsync(req.PaymentId, req.ExternalRef);
            await EventBridge.PublishAsync(new OrderStatusChangedEvent
            {
                OrderId = req.OrderId,
                Status = "confirmed"
            });
            return Ok();
        }

        [HttpPost("webhook/failed")]
        public async Task<IActionResult> PaymentFailed([FromBody] PaymentWebhookRequest req)
        {
            await _paymentService.MarkPaymentFailedAsync(req.PaymentId);
            await EventBridge.PublishAsync(new OrderStatusChangedEvent
            {
                OrderId = req.OrderId,
                Status = "payment_failed"
            });
            return Ok();
        }
    }

    // DTOs
    public class InitiatePaymentRequest
    {
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class PaymentWebhookRequest
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string ExternalRef { get; set; }
    }
} 
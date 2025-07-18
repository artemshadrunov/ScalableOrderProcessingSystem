using Implementation.Models;
using Implementation.Infrastructure;
using System;
using System.Threading.Tasks;
using Implementation;

namespace Implementation.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly OrderService _orderService;
        public PaymentService(PaymentRepository paymentRepository, OrderService orderService)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
        }

        public async Task<Payment> CreatePaymentAsync(string orderId)
        {
            // Идемпотентность: ищем уже существующий платеж для этого заказа
            var existing = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (existing != null && existing.Status != "failed" && existing.Status != "refunded")
                return existing;

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = orderId,
                Status = "initiated",
                Provider = "stripe",
                ExternalRef = null,
                CreatedAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);
            return payment;
        }

        public async Task MarkPaymentCapturedAsync(string paymentId, string externalRef)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) 
                throw new Exception("Payment not found");
            
            // Идемпотентность: если уже captured — ничего не делаем
            if (payment.Status == "captured")
                return;

            payment.Status = "captured";
            payment.ExternalRef = externalRef;
            await _paymentRepository.UpdateAsync(payment);

            // Обновляем заказ и публикуем событие
            await _orderService.UpdateOrderStatusAndPublishAsync(payment.OrderId, "confirmed");
        }

        public async Task MarkPaymentFailedAsync(string paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) 
                throw new Exception("Payment not found");
            
            // Идемпотентность: если уже failed — ничего не делаем
            if (payment.Status == "failed")
                return;

            payment.Status = "failed";
            await _paymentRepository.UpdateAsync(payment);

            // Обновляем заказ и публикуем событие
            await _orderService.UpdateOrderStatusAndPublishAsync(payment.OrderId, "payment_failed");
        }
    }
} 
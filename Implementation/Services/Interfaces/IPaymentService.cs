namespace Implementation.Services.Interfaces;

using Implementation.Models;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(string orderId);
    Task MarkPaymentCapturedAsync(string paymentId, string externalRef);
    Task MarkPaymentFailedAsync(string paymentId);
}

namespace Implementation.Infrastructure.Interfaces;

using Implementation.Models;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(string paymentId);
    Task<Payment?> GetByOrderIdAsync(string orderId);
    Task UpdateAsync(Payment payment);
}

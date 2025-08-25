using Implementation.Models;
using Implementation.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Infrastructure
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OrdersDbContext _dbContext;
        
        public PaymentRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Payment?> GetByIdAsync(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
                throw new ArgumentException("Payment ID cannot be null or empty", nameof(paymentId));

            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetByOrderIdAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                throw new ArgumentException("Order ID cannot be null or empty", nameof(orderId));

            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
        }
    }
} 
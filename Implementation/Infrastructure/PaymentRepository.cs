using Implementation.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Infrastructure
{
    public class PaymentRepository
    {
        private readonly OrdersDbContext _dbContext;
        public PaymentRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Payment payment)
        {
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Payment?> GetByIdAsync(string paymentId)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetByOrderIdAsync(string orderId)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
        }
    }
} 
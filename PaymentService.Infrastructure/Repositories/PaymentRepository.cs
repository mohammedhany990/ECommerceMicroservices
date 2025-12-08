using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;

        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Payment payment) => await _dbContext.Payments.AddAsync(payment);


        public async Task<Payment?> GetByIdAsync(Guid paymentId) => await _dbContext.Payments.FindAsync(paymentId);


        public async Task<Payment?> GetByOrderIdAsync(Guid orderId) =>
            await _dbContext.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);



        public async Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId) =>
            await _dbContext.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);


        public async Task<List<Payment>?> GetByUserIdAsync(Guid userId) =>
            await _dbContext.Payments.Where(p => p.UserId == userId).ToListAsync();


        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();


        public async Task UpdateAsync(Payment payment) => _dbContext.Payments.Update(payment);

    }
}

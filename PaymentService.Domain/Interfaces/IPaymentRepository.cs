using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid paymentId);
        Task<Payment?> GetByOrderIdAsync(Guid orderId);
        Task<List<Payment>?> GetByUserIdAsync(Guid userId);

        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);

        Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId);

        Task SaveChangesAsync();

    }
}

using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using System.Reflection;

namespace PaymentService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Payment> Payments { get; set; }
    }
}

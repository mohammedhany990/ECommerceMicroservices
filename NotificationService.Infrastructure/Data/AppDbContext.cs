using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using System.Reflection;


namespace NotificationService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


        public DbSet<Notification> Notifications { get; set; }
    }
}

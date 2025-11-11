using Microsoft.EntityFrameworkCore;
using ShippingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Infrastructure.Data
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


        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;

namespace ADAProject.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

         // Models/AppDbContext.cs
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<User> Users { get; set; }


    }
}

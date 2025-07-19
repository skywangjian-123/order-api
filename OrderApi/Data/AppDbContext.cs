using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Models;

namespace OrderApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; } // 添加 DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 Order 实体
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderId).ValueGeneratedOnAdd();

                // 配置一对多关系
                entity.HasMany(o => o.Items)
                    .WithOne()
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Orders");
            });

            // 配置 OrderItem 实体
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Id).ValueGeneratedOnAdd();

                entity.Property(oi => oi.ProductId).IsRequired();
                entity.Property(oi => oi.Quantity).IsRequired();

                entity.ToTable("OrderItems");
            });
        }
    }
}

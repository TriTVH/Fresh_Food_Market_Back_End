using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrderService_Redis.API.Entities;

public partial class OrderSerRedisContext : DbContext
{
    public OrderSerRedisContext()
    {
    }

    public OrderSerRedisContext(DbContextOptions<OrderSerRedisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:ffmser.database.windows.net,1433;Initial Catalog=OrderSer_Redis;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Status).HasMaxLength(50).HasColumnName("status");
            entity.Property(e => e.UserId).HasMaxLength(100).HasColumnName("user_id").IsRequired(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName).HasMaxLength(255).HasColumnName("product_name").IsRequired(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").HasColumnName("price");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)").HasColumnName("sub_total");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Model.DBContext;

public partial class OrderMgmtFfmContext : DbContext
{
    public OrderMgmtFfmContext()
    {
    }

    public OrderMgmtFfmContext(DbContextOptions<OrderMgmtFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<SubscriptionTicket> SubscriptionTickets { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=tcp:ffmser.database.windows.net,1433;Initial Catalog=order_mgmt_ffm;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__order__46596229DA72084E");

            entity.ToTable("order");

            entity.HasIndex(e => e.OrderNumber, "UQ__order__730E34DF5ECDF696").IsUnique();

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CancelledDate).HasColumnName("cancelled_date");
            entity.Property(e => e.ConfirmedDate).HasColumnName("confirmed_date");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_date");
            entity.Property(e => e.DeliveredDate).HasColumnName("delivered_date");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("order_number");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_status");
            entity.Property(e => e.ShippedDate).HasColumnName("shipped_date");
            entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.ShippingName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("shipping_name");
            entity.Property(e => e.ShippingPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("shipping_phone");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.Subtotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_date");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("fk_order_subscription");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__order_de__3C5A40805BE3FF37");

            entity.ToTable("order_detail");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountPerItem)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("discount_per_item");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order_detail_order");
        });

        modelBuilder.Entity<SubscriptionTicket>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__subscrip__863A7EC1601804D8");

            entity.ToTable("subscription_ticket");

            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Frequency)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("frequency");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalOrders)
                .HasDefaultValue(0)
                .HasColumnName("total_orders");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

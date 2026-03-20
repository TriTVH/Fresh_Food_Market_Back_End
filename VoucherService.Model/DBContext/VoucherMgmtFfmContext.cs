using Microsoft.EntityFrameworkCore;

namespace VoucherService.Model.DBContext;

public partial class VoucherMgmtFfmContext : DbContext
{
    public VoucherMgmtFfmContext()
    {
    }

    public VoucherMgmtFfmContext(DbContextOptions<VoucherMgmtFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Voucher> Vouchers { get; set; }
    public virtual DbSet<VoucherDetail> VoucherDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__voucher__voucher_id");

            entity.ToTable("voucher");

            entity.HasIndex(e => e.VoucherCode, "UQ__voucher__2173106967E7C1FF").IsUnique();

            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("voucher_code");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("voucher_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.TypeDiscountTime)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_discount_time");
            entity.Property(e => e.MaxUsage).HasColumnName("max_usage");
            entity.Property(e => e.CurrentUsage).HasColumnName("current_usage");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.ToDate).HasColumnName("to_date");
            entity.Property(e => e.DiscountFor)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("discount_for");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
        });

        modelBuilder.Entity<VoucherDetail>(entity =>
        {
            entity.HasKey(e => e.VoucherDetailId).HasName("PK__voucher_detail__voucher_detail_id");

            entity.ToTable("voucher_detail");

            entity.Property(e => e.VoucherDetailId).HasColumnName("voucher_detail_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            entity.Property(e => e.AppliedDate).HasColumnName("applied_date");

            entity.HasOne(d => d.Voucher)
                .WithMany(p => p.VoucherDetails)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_voucher_detail_voucher");
        });
    }
}

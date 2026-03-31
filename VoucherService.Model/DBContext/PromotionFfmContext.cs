using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VoucherService.Model.DBContext;

public partial class PromotionFfmContext : DbContext
{
    public PromotionFfmContext()
    {
    }

    public PromotionFfmContext(DbContextOptions<PromotionFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DiscountProgram> DiscountPrograms { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherDetail> VoucherDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:ffmser.database.windows.net,1433;Database=promotion_ffm;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountProgram>(entity =>
        {
            entity.HasKey(e => e.ProgramId);

            entity.ToTable("discount_program");

            entity.Property(e => e.ProgramId).HasColumnName("program_id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CurrentUsage).HasColumnName("current_usage");
            entity.Property(e => e.DiscountFor).HasColumnName("discount_for");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.DiscountProduct).HasColumnName("discount_product");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.MaxUsage).HasColumnName("max_usage");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.ToDate).HasColumnName("to_date");
            entity.Property(e => e.TypeDiscount)
                .HasMaxLength(50)
                .HasColumnName("type_discount");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.ToTable("voucher");

            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CurrentUsage).HasColumnName("current_usage");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountMax).HasColumnName("discount_max")
            .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.MaxUsage).HasColumnName("max_usage");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.ToDate).HasColumnName("to_date");
            entity.Property(e => e.TypeDiscountTime)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_discount_time");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("voucher_code");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(200)
                .IsUnicode(true)
                .HasColumnName("voucher_name");
        });

        modelBuilder.Entity<VoucherDetail>(entity =>
        {
            entity.ToTable("voucher_detail");

            entity.Property(e => e.VoucherDetailId).HasColumnName("voucher_detail_id");
            entity.Property(e => e.AppliedDate).HasColumnName("applied_date");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            entity.Ignore(e => e.DiscountAmount);

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherDetails)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_voucher_detail_voucher");

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherDetails)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_voucher_detail_voucher");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

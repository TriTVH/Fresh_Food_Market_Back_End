using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Model.DBContext;

public partial class BatchMgmtFfmContext : DbContext
{
    public BatchMgmtFfmContext()
    {
    }

    public BatchMgmtFfmContext(DbContextOptions<BatchMgmtFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<BatchDetail> BatchDetails { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=tcp:ffmser.database.windows.net,1433;Initial Catalog=batch_mgmt_ffm;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.ToTable("batch");

            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.BatchCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("batch_code");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.DeliveredDate).HasColumnName("delivered_date");
            entity.Property(e => e.ImageConfirmReceived)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_confirm_received");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalItems).HasColumnName("total_items");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Batches)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_batch_supplier");
        });

        modelBuilder.Entity<BatchDetail>(entity =>
        {
            entity.ToTable("batch_detail");

            entity.Property(e => e.BatchDetailId).HasColumnName("batch_detail_id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ExpiredDate).HasColumnName("expired_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name")
            .HasMaxLength(254)
            .IsUnicode(true);
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Batch).WithMany(p => p.BatchDetails)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_batch_detail_batch");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

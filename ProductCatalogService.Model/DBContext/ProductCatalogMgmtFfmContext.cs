using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogService.Model.DBContext;

public partial class ProductCatalogMgmtFfmContext : DbContext
{
    public ProductCatalogMgmtFfmContext()
    {
    }

    public ProductCatalogMgmtFfmContext(DbContextOptions<ProductCatalogMgmtFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=tcp:ffmser.database.windows.net,1433;Initial Catalog=product_catalog_mgmt_ffm;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_product_1");

            entity.ToTable("product");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Certification).HasColumnName("certification");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Images).HasColumnName("images");
            entity.Property(e => e.IsAvailable).HasColumnName("is_available");
            entity.Property(e => e.IsOrganic).HasColumnName("is_organic");
            entity.Property(e => e.ManufacturingLocation)
                .HasMaxLength(255)
                .HasColumnName("manufacturing_location");
            entity.Property(e => e.PriceSell)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("price_sell");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RatingAverage)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating_average");
            entity.Property(e => e.RatingCount).HasColumnName("rating_count");
            entity.Property(e => e.SoldCount).HasColumnName("sold_count");
            entity.Property(e => e.SubCategoryId).HasColumnName("sub_category_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_sub_category");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_product_supplier");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.ToTable("sub_category");

            entity.Property(e => e.SubCategoryId).HasColumnName("sub_category_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.SubCategoryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("sub_category_name");

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sub_category_category");
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
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

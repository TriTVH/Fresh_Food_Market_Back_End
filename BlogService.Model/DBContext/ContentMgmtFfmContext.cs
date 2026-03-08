using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Model.DBContext;

public partial class ContentMgmtFfmContext : DbContext
{
    public ContentMgmtFfmContext()
    {
    }

    public ContentMgmtFfmContext(DbContextOptions<ContentMgmtFfmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<News> News { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer(" Server=tcp:ffmser.database.windows.net,1433;Initial Catalog= content_mgmt_ffm;Persist Security Info=False;User ID=ffm_admin;Password=Server@1235;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<News>(entity =>
        {
            entity.ToTable("news");

            entity.Property(e => e.NewsId).HasColumnName("news_id");
           
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.PublishDate).HasColumnName("publish_date");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

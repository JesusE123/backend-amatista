using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_amatista.Models;

public partial class DbamatistaContext : DbContext
{
    public DbamatistaContext()
    {
    }

    public DbamatistaContext(DbContextOptions<DbamatistaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Categori__6DB3A68A8F9E6DDF");

            entity.Property(e => e.IdCategory).HasColumnName("ID_Category");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__522DE496DAB97971");

            entity.Property(e => e.IdProduct).HasColumnName("ID_Product");
            entity.Property(e => e.IdCategory).HasColumnName("ID_Category");
            entity.Property(e => e.Item).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__ID_Cat__534D60F1");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.IdSale).HasName("PK__Sales__2071DEA3FA3D3B0E");

            entity.Property(e => e.IdSale).HasColumnName("ID_Sale");
            entity.Property(e => e.Cuit)
                .HasMaxLength(20)
                .HasColumnName("CUIT");
            entity.Property(e => e.Customer).HasMaxLength(255);
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => e.IdSaleDetail).HasName("PK__SaleDeta__9C70F49CA4B26AA9");

            entity.Property(e => e.IdSaleDetail).HasColumnName("ID_SaleDetail");
            entity.Property(e => e.IdProduct).HasColumnName("ID_Product");
            entity.Property(e => e.IdSale).HasColumnName("ID_Sale");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SaleDetai__ID_Pr__5441852A");

            entity.HasOne(d => d.IdSaleNavigation).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.IdSale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SaleDetai__ID_Sa__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__ED4DE442718CF9E4");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534364F756C").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("ID_User");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

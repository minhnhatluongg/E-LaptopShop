using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    //Kho - Nhập xuất Sản Phẩm

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryHistory> InventoryHistories { get; set; }

    public virtual DbSet<OrderHistory> OrderHistories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierOrder> SupplierOrders { get; set; }

    public virtual DbSet<SupplierOrderItem> SupplierOrderItems { get; set; }

    public virtual DbSet<SysFile> SysFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07DBFE0BFB");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07B7B3B259");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__UserId__4E88ABD4");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC079E5D0449");

            entity.Property(e => e.CostPrice).HasDefaultValue(0);
            entity.Property(e => e.DiscountAmount).HasDefaultValue(0);
            entity.Property(e => e.DiscountPercent).HasDefaultValue(0);
            entity.Property(e => e.TaxAmount).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__5165187F");
            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Produ__52593CB8");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentT__3214EC072905DF2C");

            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions).HasConstraintName("FK__PaymentTr__Order__5FB337D6");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC078E5484EF");
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Discount).HasColumnType("decimal(5,2)");
            entity.Property(p => p.CreatedAt)
                     .HasColumnType("datetime")
                     .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.Discount).HasDefaultValue(0m);
            entity.Property(e => e.InStock).HasDefaultValue(0);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Products__Catego__4316F928");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC078C5F1344");


            // Các thuộc tính không cho phép null
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileSize).IsRequired();
            entity.Property(e => e.UploadedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.IsActive).IsRequired();

            // Các giá trị mặc định
            entity.Property(e => e.IsMain).HasDefaultValue(false);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.FileSize).HasDefaultValue(0);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Các thuộc tính có thể null
            entity.Property(e => e.AltText).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);

            // Quan hệ với bảng Product
            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductImages_Products");

            //Quan hệ với bảng SysFile
            entity.HasOne(d => d.SysFile)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.SysFileId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ProductImages_SysFiles");

            // Các index
            entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductId");
            entity.HasIndex(e => new { e.ProductId, e.IsMain }, "IX_ProductImages_ProductId_IsMain");


        });

        modelBuilder.Entity<ProductSpecification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductS__3214EC077A3B327A");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSpecifications).HasConstraintName("FK__ProductSp__Produ__49C3F6B7");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07A09B5C70");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07F0A62389");

            // Các trường bắt buộc
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);

            // Các trường có thể null
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.RefreshToken).HasMaxLength(100);
            entity.Property(e => e.VerificationToken).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Các giá trị mặc định
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.LoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.IsLocked).HasDefaultValue(false);

            // Quan hệ với bảng Role
            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");

            // Chỉ mục
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UQ__Users__A9D10534BCF0AE75");

            entity.HasIndex(e => e.RoleId)
                .HasDatabaseName("IX_Users_RoleId");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            entity.HasIndex(e => e.LastLoginAt)
                .HasDatabaseName("IX_Users_LastLoginAt");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity
            .HasIndex(x => new { x.UserId, x.IsDefault })
            .HasFilter("[IsDefault] = 1") // chỉ unique khi IsDefault = 1
            .IsUnique();
            entity.HasKey(e => e.Id).HasName("PK__UserAddr__3214EC07499E7131");
            
            entity.Property(e => e.IsDefault).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses).HasConstraintName("FK__UserAddre__UserI__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShoppingCarts__3214EC07");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ShoppingCarts_Users");
        });

        modelBuilder.Entity<ShoppingCartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShoppingCartItems__3214EC07");

            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ShoppingCart)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ShoppingCartItems_ShoppingCarts");

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ShoppingCartItems_Products");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventories__3214EC07");

            entity.Property(e => e.CurrentStock).HasDefaultValue(0);
            entity.Property(e => e.MinimumStock).HasDefaultValue(5);
            entity.Property(e => e.ReorderPoint).HasDefaultValue(10);
            entity.Property(e => e.AverageCost).HasDefaultValue(0);
            entity.Property(e => e.LastPurchasePrice).HasDefaultValue(0);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Inventories_Products");
        });

        modelBuilder.Entity<InventoryHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InventoryHistories__3214EC07");

            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Inventory)
                .WithMany(p => p.HistoryRecords)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_InventoryHistories_Inventories");
        });

        modelBuilder.Entity<OrderHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderHistories__3214EC07");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderHistories)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderHistories_Orders");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Suppliers__3214EC07");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<SupplierOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SupplierOrders__3214EC07");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_SupplierOrders_Suppliers");
        });

        modelBuilder.Entity<SupplierOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SupplierOrderItems__3214EC07");

            entity.Property(e => e.ReceivedQuantity).HasDefaultValue(0);

            entity.HasOne(d => d.SupplierOrder)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.SupplierOrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SupplierOrderItems_SupplierOrders");

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_SupplierOrderItems_Products");
        });

        modelBuilder.Entity<SysFile>(entity =>
        {
            entity.ToTable("SysFile");
            entity.HasKey(e => e.Id).HasName("PK__SysFiles__3214EC07");

            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StorageType).IsRequired().HasMaxLength(100);

            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StorageType).HasDefaultValue("local");

            entity.Property(e => e.UploadedBy).HasMaxLength(50);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

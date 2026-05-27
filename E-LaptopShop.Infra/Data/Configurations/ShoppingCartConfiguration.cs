using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCarts");

            builder.HasKey(sc => sc.Id)
                   .HasName("PK__ShoppingCarts__3214EC07");

            builder.Property(sc => sc.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(sc => sc.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(sc => sc.UpdatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.HasIndex(sc => sc.UserId).HasDatabaseName("IX_ShoppingCarts_UserId");

            builder.HasOne(sc => sc.User)
                   .WithMany(u => u.ShoppingCarts)
                   .HasForeignKey(sc => sc.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_ShoppingCarts_Users");

            builder.HasMany(sc => sc.Items)
                   .WithOne(sci => sci.ShoppingCart)
                   .HasForeignKey(sci => sci.ShoppingCartId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_ShoppingCartItems_ShoppingCarts");
        }
    }
}

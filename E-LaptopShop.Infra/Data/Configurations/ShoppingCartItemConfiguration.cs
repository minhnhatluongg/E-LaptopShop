using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.ToTable("ShoppingCartItems");

            builder.HasKey(sci => sci.Id)
                   .HasName("PK__ShoppingCartItems__3214EC07");

            builder.Property(sci => sci.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(sci => sci.Quantity).HasDefaultValue(1);

            builder.Property(sci => sci.AddedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            // ShoppingCart relationship is configured in ShoppingCartConfiguration (HasMany side)

            builder.HasOne(sci => sci.Product)
                   .WithMany(p => p.ShoppingCartItems)
                   .HasForeignKey(sci => sci.ProductId)
                   .HasConstraintName("FK_ShoppingCartItems_Products");
        }
    }
}

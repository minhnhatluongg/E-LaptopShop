using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductCommentConfiguration : IEntityTypeConfiguration<ProductComment>
    {
        public void Configure(EntityTypeBuilder<ProductComment> builder)
        {
            builder.ToTable("ProductComments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(c => c.UpdatedAt).HasColumnType("datetime");

            builder.HasIndex(c => c.ProductId).HasDatabaseName("IX_ProductComments_ProductId");
            builder.HasIndex(c => c.UserId).HasDatabaseName("IX_ProductComments_UserId");

            builder.HasOne(c => c.Product)
                   .WithMany(p => p.ProductComments)
                   .HasForeignKey(c => c.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ParentComment)
                   .WithMany()
                   .HasForeignKey(c => c.ParentCommentId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

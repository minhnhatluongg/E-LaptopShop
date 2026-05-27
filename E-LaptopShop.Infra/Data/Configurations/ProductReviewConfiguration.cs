using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.Comment).HasMaxLength(2000);

            builder.Property(pr => pr.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.HasIndex(pr => pr.ProductId).HasDatabaseName("IX_ProductReviews_ProductId");
            builder.HasIndex(pr => pr.UserId).HasDatabaseName("IX_ProductReviews_UserId");

            builder.HasOne(pr => pr.Product)
                   .WithMany(p => p.ProductReviews)
                   .HasForeignKey(pr => pr.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.User)
                   .WithMany(u => u.ProductReviews)
                   .HasForeignKey(pr => pr.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

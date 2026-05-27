using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(220).IsUnicode(false);
            builder.Property(p => p.Excerpt).HasMaxLength(500);
            builder.Property(p => p.Content).HasColumnType("nvarchar(max)");
            builder.Property(p => p.CoverImageUrl).HasMaxLength(500);
            builder.Property(p => p.Status).IsRequired().HasMaxLength(20).HasDefaultValue("DRAFT");

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(p => p.Slug)
                   .IsUnique()
                   .HasDatabaseName("UX_Posts_Slug");

            builder.HasIndex(p => new { p.Status, p.PublishedAt })
                   .HasDatabaseName("IX_Posts_Status_Published");

            builder.HasOne(p => p.Author)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.AuthorUserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

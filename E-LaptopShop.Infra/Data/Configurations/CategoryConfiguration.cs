using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id)
                   .HasName("PK_Categories");

            builder.Property(c => c.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(c => c.Slug)
                   .HasMaxLength(200)
                   .IsUnicode(false)
                   .IsRequired();

            builder.Property(c => c.Description)
                   .HasMaxLength(255);

            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);

            builder.Property(c => c.DisplayOrder)
                   .HasDefaultValue(0);

            builder.Property(c => c.MetaTitle).HasMaxLength(150);
            builder.Property(c => c.MetaDescription).HasMaxLength(300);
            builder.Property(c => c.MetaKeywords).HasMaxLength(200);

            builder.Property(c => c.CreatedBy).HasMaxLength(64);
            builder.Property(c => c.UpdatedBy).HasMaxLength(64);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            // Concurrency token
            builder.Property(c => c.RowVersion)
                   .IsRowVersion();

            // Self-reference (hierarchy)
            builder.HasOne(c => c.Parent)
                   .WithMany(c => c.Children)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Soft-delete global filter
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Indexes
            builder.HasIndex(c => new { c.ParentId, c.DisplayOrder })
                   .HasDatabaseName("IX_Categories_Parent_Display");

            builder.HasIndex(c => c.Slug)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("UX_Categories_Slug");

            builder.HasIndex(c => new { c.ParentId, c.Name })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("UX_Categories_Parent_Name");
        }
    }
}

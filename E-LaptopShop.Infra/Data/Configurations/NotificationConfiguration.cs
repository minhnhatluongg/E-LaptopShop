using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).IsRequired().HasMaxLength(150);
            builder.Property(n => n.Body).HasMaxLength(1000);
            builder.Property(n => n.Type).IsRequired().HasMaxLength(50).HasDefaultValue("INFO");
            builder.Property(n => n.Url).HasMaxLength(255);
            builder.Property(n => n.IsRead).HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(n => new { n.UserId, n.IsRead })
                   .HasDatabaseName("IX_Notifications_User_Unread");

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

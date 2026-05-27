using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.EventType).IsRequired().HasMaxLength(50);
            builder.Property(al => al.SessionId).HasMaxLength(64);
            builder.Property(al => al.IpAddress).HasMaxLength(45);
            builder.Property(al => al.Metadata).HasColumnType("nvarchar(max)");

            builder.Property(al => al.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(al => new { al.UserId, al.EventType, al.CreatedAt })
                   .IsDescending(false, false, true)
                   .HasDatabaseName("IX_ActivityLogs_User_Event");

            builder.HasOne(al => al.User)
                   .WithMany(u => u.ActivityLogs)
                   .HasForeignKey(al => al.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

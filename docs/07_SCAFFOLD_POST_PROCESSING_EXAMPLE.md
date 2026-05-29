# Hướng dẫn xử lý sau scaffold — ví dụ entity `Coupon`

> Mỗi entity scaffolded cần 3 việc:
>   1. Entity POCO trong `Domain/Entities/`
>   2. `IEntityTypeConfiguration<T>` trong `Infra/Data/Configurations/`
>   3. Thêm `DbSet<T>` vào `ApplicationDbContext`

## Đầu vào — scaffold output (`Domain/Entities/Scaffolded/Coupons.cs`)

```csharp
using System;
using System.Collections.Generic;

namespace E_LaptopShop.Infra;     // ❌ sai namespace

public partial class Coupons       // ❌ tên class số nhiều, sai convention
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int? UsageLimitPerUser { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public virtual ICollection<CouponUsages> CouponUsages { get; set; } = new List<CouponUsages>();
    // ❌ tham chiếu CouponUsages (số nhiều) — sẽ phải sửa sau khi rename
}
```

## Output 1 — `Domain/Entities/Coupon.cs`

```csharp
using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;   // ✅ đúng namespace

public class Coupon                        // ✅ số ít, bỏ partial nếu không cần
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = null!;   // PERCENT | AMOUNT
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int? UsageLimitPerUser { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    // Navigation
    public virtual ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();
    // ✅ tham chiếu CouponUsage (số ít)
}
```

## Output 2 — `Infra/Data/Configurations/CouponConfiguration.cs`

```csharp
using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations;

public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> b)
    {
        b.ToTable("Coupons", t =>
            t.HasCheckConstraint(
                "CK_Coupons_DiscountType",
                "DiscountType IN (N'PERCENT', N'AMOUNT')"));

        b.HasKey(c => c.Id);

        b.Property(c => c.Code)
         .IsRequired()
         .HasMaxLength(50);

        b.Property(c => c.Description).HasMaxLength(255);

        b.Property(c => c.DiscountType)
         .IsRequired()
         .HasMaxLength(20)
         .HasDefaultValue("PERCENT");

        b.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
        b.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        b.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");

        b.Property(c => c.UsedCount).HasDefaultValue(0);

        b.Property(c => c.StartsAt).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(c => c.IsActive).HasDefaultValue(true);

        b.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(c => c.CreatedBy).HasMaxLength(100);

        // Index
        b.HasIndex(c => c.Code)
         .IsUnique()
         .HasDatabaseName("UX_Coupons_Code");
    }
}
```

## Output 3 — thêm vào `Infra/Data/ApplicationDbContext.cs`

```csharp
public virtual DbSet<Coupon> Coupons { get; set; }
public virtual DbSet<CouponUsage> CouponUsages { get; set; }
public virtual DbSet<ProductTag> ProductTags { get; set; }
// ...
```

## Checklist sau khi paste mỗi entity

- [ ] Namespace đã đổi sang `E_LaptopShop.Domain.Entities`
- [ ] Class name đã đổi sang số ít (Coupons → Coupon)
- [ ] File name khớp class name (Coupons.cs → Coupon.cs)
- [ ] Bỏ `partial` nếu không có file partial thứ 2
- [ ] Navigation properties tham chiếu **entity số ít** (`ICollection<CouponUsage>` chứ không `ICollection<CouponUsages>`)
- [ ] Tạo Configuration file tương ứng trong `Infra/Data/Configurations/`
- [ ] Copy decimal precision (`HasColumnType("decimal(18,2)")`) cho mọi money column
- [ ] Copy index, unique, default value từ ScaffoldDbContext
- [ ] **Tự thêm** check constraint nếu có (scaffold KHÔNG đọc được)
- [ ] Thêm `DbSet<T>` vào ApplicationDbContext
- [ ] Nếu entity có FK đến entity cũ (User, Product...) → thêm inverse navigation vào entity cũ
- [ ] Build project — KHÔNG còn lỗi
- [ ] Sau khi tất cả entity đã xong → xóa folder `Scaffolded/` (cả Domain và Infra)

## Mẹo VS

- **Ctrl+R, R**: Rename symbol (đổi tên class + tự update mọi reference + rename file).
- **Ctrl+.**: Quick action (gợi ý thêm `using`, fix namespace).
- **Ctrl+K, Ctrl+D**: Format document — chạy sau khi paste.
- **Solution Explorer → Click "Sync namespace"**: VS 17.6+ tự sync namespace theo folder.

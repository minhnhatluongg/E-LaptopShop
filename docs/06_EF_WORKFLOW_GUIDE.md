# HD đồng bộ EF Core với Database — DB-first / Code-first / Hybrid

> Tài liệu này giải đáp: **khi bạn đã add table mới ở SQL (file `02_schema_upgrade.sql`), làm sao để có code C# tương ứng (entity + DbSet) đồng bộ?**

---

## 1. Ba workflow chính

| Workflow | Source of truth | Tool | Khi nào dùng |
| --- | --- | --- | --- |
| **DB-first (scaffold)** | Database thật | `dotnet ef dbcontext scaffold` | DB đã có sẵn (legacy), bạn muốn sinh code từ DB. Phù hợp với project bạn vì đã chạy SQL scripts. |
| **Code-first (migrations)** | C# Entity + Configuration | `dotnet ef migrations add` + `database update` | Bắt đầu mới hoặc bạn muốn dev quản entity → DB tự cập nhật theo |
| **Hybrid** | Mix | Cả hai | DB đã có, scaffold lần đầu, sau đó tiếp tục bằng migration |

Project bạn hiện đang dùng **Code-first style** (entities viết tay) nhưng **chưa có migration nào** — DB đã được tạo bằng SQL script. Đây là **hybrid implicit**.

---

## 2. DB-first — scaffold từ Azure SQL (cách bạn cần)

### 2.1 Chuẩn bị

Cài 2 NuGet packages vào `E-LaptopShop.Infra`:

```bash
cd D:\project-building-CV\laptop_shop\E-LaptopShop\E-LaptopShop.Infra
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Cài CLI tool (1 lần / máy):

```bash
dotnet tool install --global dotnet-ef
# nếu đã cài: dotnet tool update --global dotnet-ef
```

Verify:

```bash
dotnet ef --version
```

### 2.2 Lệnh scaffold cơ bản — scaffold TOÀN BỘ DB

```bash
# Chạy ở thư mục solution gốc
cd D:\project-building-CV\laptop_shop\E-LaptopShop

dotnet ef dbcontext scaffold ^
  "Server=tcp:be-laptopshop.database.windows.net,1433;Initial Catalog=free-sql-db-9719142;User ID=admin_laptopshop;Password=<password-mới>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;" ^
  Microsoft.EntityFrameworkCore.SqlServer ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop ^
  --context ApplicationDbContext ^
  --context-dir Data\Scaffolded ^
  --output-dir ..\E-LaptopShop.Domain\Entities\Scaffolded ^
  --use-database-names ^
  --no-onconfiguring
```

> ⚠️ **KHÔNG chạy lệnh này lên project hiện tại** vì sẽ overwrite mọi file `*.cs`. Lúc đầu, scaffold vào folder con `Scaffolded/` rồi so sánh, copy phần cần sang file thật.

**Giải thích flags**:

| Flag | Ý nghĩa |
| --- | --- |
| `<connection string>` | Connection tới Azure SQL |
| `Microsoft.EntityFrameworkCore.SqlServer` | Provider name |
| `--project` | Project chứa entities & DbContext sẽ sinh ra |
| `--startup-project` | Project có Program.cs (để EF đọc appsettings/user-secrets) |
| `--context` | Tên DbContext class |
| `--context-dir` | Thư mục con (trong --project) chứa DbContext |
| `--output-dir` | Thư mục chứa entities |
| `--use-database-names` | Giữ tên column/table giống DB, không tự đổi sang PascalCase |
| `--no-onconfiguring` | KHÔNG sinh `OnConfiguring` (vì đã có connection string từ DI) |

Các flag hữu ích khác:

| Flag | Khi nào dùng |
| --- | --- |
| `--table Coupons --table Wishlists` | Chỉ scaffold các table cụ thể |
| `--schema dbo` | Chỉ scaffold schema `dbo` |
| `--data-annotations` | Sinh entities với `[Required]`, `[Key]` (không khuyến nghị nếu muốn POCO thuần) |
| `--force` | Overwrite file đã có (NGUY HIỂM) |

### 2.3 Scaffold CHỈ các table mới (workflow phù hợp với bạn)

Vì bạn vừa add 20 table upgrade (Coupons, Wishlists, LoyaltyTiers...) qua SQL script, làm như sau:

```bash
cd D:\project-building-CV\laptop_shop\E-LaptopShop

dotnet ef dbcontext scaffold ^
  "Name=DefaultConnection" ^
  Microsoft.EntityFrameworkCore.SqlServer ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop ^
  --context ScaffoldDbContext ^
  --context-dir Data\Scaffolded ^
  --output-dir ..\E-LaptopShop.Domain\Entities\Scaffolded ^
  --table Coupons ^
  --table CouponUsages ^
  --table ProductTags ^
  --table ProductTagMap ^
  --table Wishlists ^
  --table Notifications ^
  --table AuditLogs ^
  --table ActivityLogs ^
  --table LoyaltyTiers ^
  --table UserLoyalty ^
  --table PointTransactions ^
  --table ReturnRequests ^
  --table RefundTransactions ^
  --table Banners ^
  --table Posts ^
  --table ContactMessages ^
  --table ProductAttributes ^
  --table ProductAttributeValues ^
  --table ProductVariants ^
  --table ProductVariantValueMap ^
  --use-database-names ^
  --no-onconfiguring ^
  --no-pluralize
```

→ EF sẽ:
1. Connect Azure SQL với connection string `DefaultConnection` (đọc từ User Secrets / appsettings).
2. Đọc schema của 20 table chỉ định.
3. Sinh ra file entity tương ứng trong `E-LaptopShop.Domain/Entities/Scaffolded/`.
4. Sinh `ScaffoldDbContext.cs` trong `E-LaptopShop.Infra/Data/Scaffolded/` (để bạn **so sánh** với `ApplicationDbContext` thật).

### 2.4 Sau khi scaffold

Bạn nhận được:
- 20 file `Coupon.cs`, `Wishlist.cs`, ... trong `Domain/Entities/Scaffolded/`.
- 1 file `ScaffoldDbContext.cs` chứa fluent config cho 20 table đó.

**Bước tiếp theo (manual, không có shortcut)**:

1. **Mỗi entity scaffold ra** → copy sang `E-LaptopShop.Domain/Entities/` (folder gốc), **xoá các EF attributes** (`[Key]`, `[Required]`, `[Column]`) để giữ POCO thuần.
2. **Cho mỗi entity** → tạo file `XxxConfiguration.cs` trong `E-LaptopShop.Infra/Data/Configurations/` theo pattern của `BrandConfiguration.cs`.
3. Trong `ApplicationDbContext` → thêm `DbSet<Coupon> Coupons { get; set; }`, ... cho 20 entity.
4. Xoá folder `Scaffolded/` (đã hết tác dụng).

**Không có tool nào tự làm bước 1-4** vì nó là quyết định kiến trúc — bạn phải quyết entity có nên là POCO không, có dùng partial class không, có tách subfolder không.

### 2.5 Lưu ý quan trọng khi scaffold

- **Mất navigation tuỳ chỉnh**: nếu code cũ có navigation `User.Orders` (1:n) mà DB chỉ thấy FK → scaffold sinh ra navigation **chưa chắc đặt tên giống** code cũ. Phải sửa tay.
- **Mất computed property**: `User.FullName => $"{FirstName} {LastName}"` không có trong DB → scaffold sẽ không sinh ra. Phải copy lại.
- **Mất `[NotMapped]`**: scaffold không biết property nào nên ignore.
- **Connection string với password** không bao giờ commit. Dùng `"Name=DefaultConnection"` để đọc từ User Secrets.

---

## 3. Code-first — cách "chuyên nghiệp" hơn

Nếu bạn muốn "DB chạy theo Entity", workflow là:

### 3.1 Thêm entity mới trong code

```csharp
// E-LaptopShop.Domain/Entities/Coupon.cs
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    // ...
}
```

### 3.2 Thêm Configuration

```csharp
// E-LaptopShop.Infra/Data/Configurations/CouponConfiguration.cs
public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> b)
    {
        b.ToTable("Coupons");
        b.Property(c => c.Code).IsRequired().HasMaxLength(50);
        b.HasIndex(c => c.Code).IsUnique();
        // ...
    }
}
```

### 3.3 Thêm DbSet vào DbContext

```csharp
public DbSet<Coupon> Coupons { get; set; }
```

### 3.4 Sinh migration

```bash
cd D:\project-building-CV\laptop_shop\E-LaptopShop

dotnet ef migrations add Add_Coupons_Table ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop ^
  --output-dir Data\Migrations
```

→ Sinh ra file `20260527_Add_Coupons_Table.cs` (timestamp + tên) trong `Infra/Data/Migrations/`. File này chứa code C# diff giữa snapshot trước và sau.

### 3.5 Apply migration lên DB

```bash
dotnet ef database update ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop
```

→ EF connect Azure SQL, chạy SQL diff, update DB. Cũng update bảng `__EFMigrationsHistory` để track đã chạy migration nào.

### 3.6 Lệnh khác

| Lệnh | Mục đích |
| --- | --- |
| `dotnet ef migrations list` | Liệt kê migration |
| `dotnet ef migrations remove` | Bỏ migration cuối (nếu chưa apply) |
| `dotnet ef migrations script` | Sinh SQL script từ migration (để chạy tay) |
| `dotnet ef database update <MigrationName>` | Rollback / forward đến migration cụ thể |
| `dotnet ef database update 0` | Drop tất cả schema (về initial) |
| `dotnet ef migrations script From To` | Sinh script SQL giữa 2 migration (deploy production thường dùng) |
| `dotnet ef dbcontext info` | In thông tin context (provider, connection...) |

---

## 4. Workflow đề xuất cho project của bạn

### Tình huống hiện tại

- DB Azure đã có 41 tables (sau khi chạy `01_*.sql` + `02_*.sql`).
- Code có **21 entity gốc**, chưa có entity cho 20 table upgrade.
- Chưa có migration folder.

### Khuyến nghị: làm Code-first từ đây, kết hợp baseline migration

```
┌─────────────────────────────────────────────────────────────┐
│ Bước 1: Scaffold 20 table mới → reference                  │
│   → folder Scaffolded/ tạm                                  │
│                                                             │
│ Bước 2: Copy entity sang Domain/Entities/, làm POCO        │
│         + tạo Configuration trong Infra/Data/Configurations│
│                                                             │
│ Bước 3: Thêm DbSet vào ApplicationDbContext                │
│                                                             │
│ Bước 4: TẠO BASELINE MIGRATION (= state hiện tại)          │
│   dotnet ef migrations add Init                             │
│   → sinh migration mô tả 41 tables                          │
│                                                             │
│ Bước 5: MARK migration "applied" KHÔNG chạy thật           │
│   (vì DB đã có sẵn schema rồi)                              │
│   Cách 1: UPDATE __EFMigrationsHistory bằng tay            │
│   Cách 2: dotnet ef migrations script + chạy đoạn cuối     │
│                                                             │
│ Bước 6: Từ giờ trở đi → Code-first thuần                   │
│   - Thêm entity → migrations add → database update          │
│   - Mọi thay đổi DB đều qua migration                      │
└─────────────────────────────────────────────────────────────┘
```

### Lệnh chi tiết cho Bước 4-5 (baseline)

```bash
# 4. Tạo migration baseline
dotnet ef migrations add Init_Baseline ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop ^
  --output-dir Data\Migrations

# 5a. Sinh SQL script tương ứng
dotnet ef migrations script 0 Init_Baseline ^
  --project E-LaptopShop.Infra ^
  --startup-project E-LaptopShop ^
  --output baseline.sql

# 5b. Mở baseline.sql, chỉ giữ phần CUỐI cùng:
#     INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
#     VALUES ('<timestamp>_Init_Baseline', '9.x.x');
#     → Chạy đoạn đó trên Azure SQL.

# 5c. Verify
dotnet ef migrations list --project E-LaptopShop.Infra --startup-project E-LaptopShop
# Output: 20260527_Init_Baseline (Applied) ✓
```

Từ giờ:

```bash
# Thêm entity mới
dotnet ef migrations add Add_NewFeature ^
  --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef database update ^
  --project E-LaptopShop.Infra --startup-project E-LaptopShop
```

---

## 5. So sánh nhanh DB-first vs Code-first

|  | DB-first (scaffold) | Code-first (migrations) |
| --- | --- | --- |
| **Source of truth** | Database | C# code |
| **Khi nào dùng** | Legacy DB, multi-team (DBA quản DB) | Greenfield, dev solo quản DB |
| **Output mỗi lần thay đổi** | Re-scaffold → có thể overwrite custom code | Migration file (versioned, có thể commit Git) |
| **Track lịch sử thay đổi** | Khó (DB schema không versioned) | Dễ (Git track migration files) |
| **Khả năng rollback** | Không có | `database update <PreviousMigration>` |
| **Customization** | Bị overwrite mỗi lần scaffold lại | Persistence — không bị overwrite |
| **Phù hợp với CV** | Không show được nhiều skill | **Show được**: kế hoạch migration, versioning, rollback |

→ **Khuyến nghị cho CV**: dùng Code-first (Migration approach). Lúc đó:
- Có folder `Migrations/` trong repo → recruiter thấy ngay bạn hiểu EF.
- Có thể demo: "Tôi add column → migration → apply" trong 1 phút.

---

## 6. Naming & convention

- Migration name: `<verb>_<noun>` PascalCase. Vd: `Add_Coupons`, `Rename_User_Phone_To_PhoneNumber`, `Drop_Obsolete_LegacyTable`.
- Configuration file: `<EntityName>Configuration.cs`. EF auto-detect bằng `ApplyConfigurationsFromAssembly`.
- DbSet property: PascalCase plural. Vd: `Coupons`, `ProductTags`.

---

## 7. Troubleshooting

| Lỗi | Nguyên nhân | Fix |
| --- | --- | --- |
| `Unable to create an object of type 'ApplicationDbContext'` | EF không tìm thấy startup project / connection string | Thêm `--startup-project E-LaptopShop` và đảm bảo User Secrets có `DefaultConnection` |
| `The migration ... has already been applied to the database` | `__EFMigrationsHistory` đã có row đó | OK — đã chạy rồi. Nếu muốn replay → xoá row history rồi update |
| `Cannot scaffold ... because of duplicate table` | Có table cùng tên ở schema khác | Thêm `--schema dbo` |
| Scaffold trả về tên table khác mong đợi (vd "tbl_X" → "TblX") | `--use-database-names` chưa bật | Bật flag đó |
| `No project was found. Change the current working directory ...` | Chạy lệnh sai folder | `cd` vào solution root rồi chạy với `--project` |
| `Could not load file or assembly 'Microsoft.EntityFrameworkCore.Design'` | Chưa cài `EntityFrameworkCore.Design` ở startup project | `dotnet add package Microsoft.EntityFrameworkCore.Design` ở project `E-LaptopShop` |
| Migration sinh ra rỗng | EF không thấy thay đổi nào | Check DbSet đã add chưa, Configuration đã được apply chưa |

---

## 8. Best practices

1. **Mỗi PR = 1 migration trở xuống**. Đừng commit 5 migrations cùng lúc.
2. **Đặt tên migration mô tả ý định**, không phải tên technical.
3. **Review migration trước khi apply** — đặc biệt `Drop column / Drop table`.
4. **Test migration trên local DB trước**, không chạy thẳng lên Azure.
5. **Khi production**: dùng `dotnet ef migrations script From To` để sinh SQL, review, DBA chạy tay → không để app tự `database update` (tránh race condition lúc deploy multi-instance).
6. **Commit `Migrations/` folder vào Git** — đây là tài sản, không phải build artifact.
7. **Không sửa migration đã apply** trên môi trường khác. Tạo migration mới để fix.

---

## 9. Cheat-sheet lệnh hay dùng

```bash
# Install
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design

# DB-first (scaffold)
dotnet ef dbcontext scaffold "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer \
  --project E-LaptopShop.Infra --startup-project E-LaptopShop \
  --output-dir ../E-LaptopShop.Domain/Entities/Scaffolded \
  --context-dir Data/Scaffolded --no-onconfiguring --use-database-names

# Code-first (migrations)
dotnet ef migrations add <Name> --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef database update --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef migrations list --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef migrations remove --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef migrations script --project E-LaptopShop.Infra --startup-project E-LaptopShop --output deploy.sql

# Info
dotnet ef dbcontext info --project E-LaptopShop.Infra --startup-project E-LaptopShop
dotnet ef dbcontext list --project E-LaptopShop.Infra --startup-project E-LaptopShop
```

---

## 10. Liên hệ với cấu trúc project của bạn

Sau lần refactor này, project có:

```
E-LaptopShop.Domain/
└── Entities/
    ├── Brand.cs                    ← POCO (đã refactor)
    ├── Category.cs                 ← POCO (đã refactor)
    ├── Product.cs                  ← POCO (đã refactor)
    ├── User.cs                     ← POCO (đã refactor)
    ├── Order.cs                    ← POCO (đã refactor)
    ├── Inventory.cs                ← VẪN CÓ attributes (chưa refactor)
    ├── ... (16 entity khác)
    └── Scaffolded/                 ← (tạm) sau khi scaffold table mới

E-LaptopShop.Infra/
└── Data/
    ├── ApplicationDbContext.cs
    ├── Configurations/             ← Fluent API config
    │   ├── BrandConfiguration.cs
    │   ├── CategoryConfiguration.cs
    │   ├── ProductConfiguration.cs
    │   ├── UserConfiguration.cs
    │   ├── OrderConfiguration.cs
    │   └── ... (sau này thêm)
    ├── Migrations/                 ← (sẽ sinh khi chạy `migrations add`)
    └── Scaffolded/                 ← (tạm) DbContext scaffold-only để so sánh
```

Khi đã tạo Configuration file cho 1 entity → entity đó không cần attribute nào nữa. Code sạch, tách rõ Domain vs Infra.

---

Có cần demo concrete step-by-step cho 1 table cụ thể (ví dụ `Coupons`), ping tôi.

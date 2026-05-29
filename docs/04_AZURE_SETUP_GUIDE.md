# HD Migrate E-LaptopShop lên Azure SQL Database

> Tài liệu này hướng dẫn từng bước để move database từ local SQL Server (`DESKTOP-VHQHJQ5\LaptopStoreDB`) lên **Azure SQL Database** (`be-laptopshop / free-sql-db-9719142`), đồng thời cấu hình project .NET 9 để chạy đa môi trường.

---

## 0. Tổng quan kiến trúc đích

```
┌─────────────────────────────────────────────────────────────┐
│  Local dev    →   Azure SQL (Southeast Asia)                │
│  EF Core 9    →   be-laptopshop.database.windows.net        │
│  ASP.NET 9    →   LaptopStoreDB (Free General Purpose)      │
└─────────────────────────────────────────────────────────────┘
```

Resource đã có trong screenshot:
- **SQL Server**: `be-laptopshop`
- **Database**: `free-sql-db-9719142` (Free GP serverless, Southeast Asia)
- **Subscription**: Azure subscription 1
- **Tài khoản**: cusocisme@gmail.com

---

## 1. Chuẩn bị Azure SQL Server

### 1.1 Cấu hình firewall

Trên Azure Portal:
1. Vào SQL server `be-laptopshop` → **Networking**.
2. Bật `Allow Azure services and resources to access this server` (nếu sau này deploy lên App Service).
3. Add firewall rule:
   - `+ Add your client IPv4 address` → đặt tên `My-Dev-Machine`.
4. Save.

> Khi đi laptop sang chỗ khác, IP đổi → quay lại đây thêm rule mới. Hoặc dùng `0.0.0.0 → 255.255.255.255` (KHÔNG khuyến nghị, chỉ cho lab).

### 1.2 Tạo SQL Admin login

Khi tạo server lần đầu, Azure đã ép bạn nhập admin login. Nếu quên, vào:
- `be-laptopshop` → **Reset password**.

Ghi nhớ:
- Server: `be-laptopshop.database.windows.net`
- Port: `1433`
- Auth: SQL Authentication
- User: ví dụ `sqladmin`
- Password: `<your password>`

### 1.3 (Tuỳ chọn) Tạo Azure AD admin

`be-laptopshop` → **Microsoft Entra ID** → Set admin = `hau.nguyen@lotviet.com`. Cho phép login bằng Azure AD thay vì user/password — đẹp cho prod.

---

## 2. Tạo database (nếu chưa có)

DB `free-sql-db-9719142` đã tồn tại. Nếu muốn tạo DB tên đẹp hơn (`LaptopStoreDB`):

1. `be-laptopshop` → **+ Create database**.
2. Database name: `LaptopStoreDB`.
3. Compute + storage: **General Purpose - Serverless - Free** (1 vCore, 32 GB).
4. Backup storage redundancy: `Locally-redundant`.
5. Networking → Public endpoint, add IP.
6. Review + Create.

> Free tier: 100K vCore-seconds/tháng (~32 giờ chạy liên tục) + 32 GB. Đủ rộng cho side project.

---

## 3. Deploy schema lên Azure

### Cách 1: Azure Portal Query Editor (đơn giản nhất)

1. Vào DB `LaptopStoreDB` → **Query editor (preview)**.
2. Login với SQL admin.
3. Mở file `01_schema_current.sql` (trong folder `Database/`) → paste → Run.
4. Tiếp tục với `02_schema_upgrade.sql`.

### Cách 2: SSMS / Azure Data Studio

```text
Server name:      be-laptopshop.database.windows.net
Authentication:   SQL Server Authentication
Login:            sqladmin
Password:         ********
```

Sau khi connect, mở file SQL → F5.

### Cách 3: sqlcmd

```bash
sqlcmd -S be-laptopshop.database.windows.net \
       -d LaptopStoreDB \
       -U sqladmin -P "<password>" \
       -i 01_schema_current.sql
```

---

## 4. Migrate data từ local lên Azure

### Khuyến nghị: dùng BACPAC

1. **Trên local (SSMS)**:
   - Connect tới `DESKTOP-VHQHJQ5`.
   - Right-click `LaptopStoreDB` → **Tasks → Export Data-tier Application**.
   - Lưu file `LaptopStoreDB.bacpac`.

2. **Upload BACPAC lên Azure Blob Storage**:
   - Tạo storage account tạm: `stlaptopshop`.
   - Container: `bacpac`.
   - Upload file.

3. **Import vào Azure SQL**:
   - Vào server `be-laptopshop` → **Import database**.
   - Chọn storage account / blob / file.
   - DB name: `LaptopStoreDB` (nếu đã tồn tại, delete trước, hoặc đổi tên).
   - SQL admin login → Create.
   - Đợi 5-15 phút.

> Nếu chỉ muốn schema (không cần data cũ): bỏ qua bước này, chỉ chạy file `01_*.sql` + `02_*.sql`.

### Lựa chọn khác: SqlPackage CLI

```bash
# Export local DB → .bacpac
SqlPackage /Action:Export \
  /SourceServerName:DESKTOP-VHQHJQ5 \
  /SourceDatabaseName:LaptopStoreDB \
  /TargetFile:"C:\Backup\LaptopStoreDB.bacpac"

# Import lên Azure
SqlPackage /Action:Import \
  /SourceFile:"C:\Backup\LaptopStoreDB.bacpac" \
  /TargetServerName:be-laptopshop.database.windows.net \
  /TargetDatabaseName:LaptopStoreDB \
  /TargetUser:sqladmin \
  /TargetPassword:"<password>"
```

---

## 5. Cấu hình project .NET 9

### 5.1 Connection string

Mở `E-LaptopShop/appsettings.json` — KHÔNG commit secret. Thay vì lưu plain text, dùng **User Secrets** (dev) hoặc **Azure Key Vault** (prod).

#### appsettings.json (giữ template, không có secret)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

#### appsettings.Development.json (KHÔNG commit — đã trong .gitignore)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:be-laptopshop.database.windows.net,1433;Initial Catalog=LaptopStoreDB;Persist Security Info=False;User ID=sqladmin;Password=<your-password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

#### Hoặc dùng dotnet user-secrets (KHUYẾN NGHỊ)

```bash
cd E-LaptopShop
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=tcp:be-laptopshop.database.windows.net,1433;Initial Catalog=LaptopStoreDB;User ID=sqladmin;Password=<password>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### 5.2 Connection string cho Local (giữ luôn để switch dễ)

Trong `appsettings.json` bạn có thể giữ 2 connection và switch qua env var:

```json
{
  "ConnectionStrings": {
    "Local":   "Server=DESKTOP-VHQHJQ5;Database=LaptopStoreDB;Integrated Security=True;TrustServerCertificate=True;",
    "Azure":   ""
  },
  "ActiveConnection": "Azure"
}
```

Trong `Program.cs`:

```csharp
var activeConn = builder.Configuration["ActiveConnection"] ?? "Local";
var connString = builder.Configuration.GetConnectionString(activeConn);
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(connString, sql =>
    {
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);   // QUAN TRỌNG: retry cho Azure SQL
    }));
```

> `EnableRetryOnFailure` rất quan trọng vì Azure SQL có throttling (đặc biệt với serverless free tier khi đang `pause`).

### 5.3 Serverless auto-pause

Azure SQL Free tier dùng **serverless** — sẽ tự pause sau ~1h không có query. Khi pause, query đầu tiên sẽ chờ 10-30 giây để wake-up. Vì vậy:
- Retry như trên là bắt buộc.
- Lần đầu request sau khi đi ngủ sẽ chậm — không phải bug.

---

## 6. Áp dụng EF Migrations (nếu dùng code-first)

Project hiện đang **không có migration folder** (tôi không thấy `/Migrations`). Nếu bạn muốn EF tự tạo schema thay vì dùng SQL script:

```bash
# Cài tool EF (1 lần)
dotnet tool install --global dotnet-ef

# Tạo migration đầu tiên (chạy ở root solution)
dotnet ef migrations add InitialCreate \
  --project E-LaptopShop.Infra \
  --startup-project E-LaptopShop

# Apply lên DB hiện tại (theo ActiveConnection)
dotnet ef database update \
  --project E-LaptopShop.Infra \
  --startup-project E-LaptopShop
```

> Nhược điểm khi dùng EF migration với DB đã có data: phải scaffold lại. Vì vậy với project legacy của bạn, **dùng SQL script `01_*.sql` + `02_*.sql` là an toàn hơn**.

---

## 7. Hardening cho production

### 7.1 Bí mật → Key Vault

```bash
# Tạo key vault
az keyvault create -n kv-laptopshop -g <rg> -l southeastasia

# Lưu connection string
az keyvault secret set --vault-name kv-laptopshop \
  --name "ConnectionStrings--DefaultConnection" \
  --value "Server=tcp:be-laptopshop..."
```

Trong `Program.cs`:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri("https://kv-laptopshop.vault.azure.net/"),
    new DefaultAzureCredential());
```

### 7.2 Managed Identity (đỉnh nhất, không cần password)

1. Bật **System Assigned Managed Identity** trên App Service.
2. `be-laptopshop` → Microsoft Entra ID → Add identity của App Service làm user.
3. Cấp role `db_datareader`, `db_datawriter` cho identity đó.
4. Connection string không còn password:

```text
Server=tcp:be-laptopshop.database.windows.net,1433;Initial Catalog=LaptopStoreDB;Authentication=Active Directory Default;Encrypt=True;
```

### 7.3 Backup tự động (đã enabled mặc định)

Azure SQL tự PITR (point-in-time restore) tới 7 ngày. Muốn longer:
- DB → **Backups** → set Long-term retention policy (vd: weekly 4 weeks, monthly 12 months).

### 7.4 Alerts

Tạo alert khi:
- vCore-seconds dùng > 80% free quota.
- Storage > 28GB (gần hết 32GB free).
- Deadlock spike.

`be-laptopshop` DB → **Alerts → + Create alert rule**.

---

## 8. Migration plan tổng quát (từ local lên Azure)

```
┌──────────┐    BACPAC      ┌──────────┐
│  Local   │ ────────────▶  │  Blob    │
│  SQL     │   Export       │ Storage  │
└──────────┘                └────┬─────┘
                                 │ Import
                                 ▼
                          ┌──────────────┐
                          │  Azure SQL   │
                          │ LaptopStoreDB│
                          └──────┬───────┘
                                 │
                                 ▼
                       Chạy 02_schema_upgrade.sql
                                 │
                                 ▼
                       Update connection string
                                 │
                                 ▼
                       App .NET 9 ✓ ready
```

---

## 9. Checklist trước khi đóng task

- [ ] DB Azure đã có 21 tables gốc (chạy script 01).
- [ ] DB đã có thêm tables upgrade (chạy script 02).
- [ ] Đã import data (BACPAC) hoặc seed master data.
- [ ] Firewall đã add IP máy dev.
- [ ] Connection string đã chuyển sang user-secrets / Key Vault.
- [ ] `EnableRetryOnFailure` đã bật trong `Program.cs`.
- [ ] Test login API + một query Products thành công.
- [ ] Backup retention đã set ≥ 7 ngày.
- [ ] (Optional) Đã viết README mới cho repo nói rõ cách switch local/azure.

---

## 10. Troubleshooting

| Triệu chứng | Nguyên nhân | Fix |
| --- | --- | --- |
| `Cannot open server '...' requested by the login` | Firewall chặn IP | Add client IP trên Azure Networking |
| `Timeout expired. Login failed.` | DB đang auto-pause | Đợi 30s, retry; bật `EnableRetryOnFailure` |
| `Reached the maximum number of databases` | Server có quá 1 free DB | Free tier mỗi subscription chỉ 1 DB free |
| EF migration `column already exists` | Schema đã có từ SQL script | Skip EF migration, dùng SQL trực tiếp |
| Slow query đầu tiên ~20s | Serverless cold start | Bình thường — pre-warm bằng cron ping nếu cần |

---

## 11. File trong folder `Database/`

| File | Mô tả |
| --- | --- |
| `00_ERD_diagram.md` | Sơ đồ ERD + tóm tắt nhóm bảng |
| `01_schema_current.sql` | Tạo lại schema hiện tại (21 tables) |
| `02_schema_upgrade.sql` | Bổ sung bảng nâng cao (Phase 2) |
| `03_data_migration_template.sql` | Hướng dẫn 4 phương án migrate data + script kiểm tra |
| `04_AZURE_SETUP_GUIDE.md` | File này — hướng dẫn tổng |

---

Có câu hỏi gì cụ thể về bước nào, cứ ping lại.

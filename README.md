# E-LaptopShop — Backend API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-Azure-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

**E-commerce API cho cửa hàng laptop. Clean Architecture · CQRS (MediatR) · JWT · IIS**

🌐 **Production:** `https://be-shopminhnhat.win-tech.vn`  
📖 **Swagger:** `https://be-shopminhnhat.win-tech.vn/` (root path)  
🖥️ **Frontend:** `http://be-shopminhnhat.click`

</div>

---

## Kiến trúc

```
E-LaptopShop/
├── E-LaptopShop/              ← Presentation layer (Controllers, Program.cs)
├── E-LaptopShop.Application/  ← Application layer (Features/CQRS, Services, DTOs, Mappings)
├── E-LaptopShop.Domain/       ← Domain layer (Entities, Repositories interfaces, Enums)
├── E-LaptopShop.Infra/        ← Infrastructure layer (DbContext, Repos, Migrations)
├── Database/                  ← SQL scripts (schema, seed data, migrations)
└── docs/                      ← Setup guides và documentation
```

## Tech Stack

| Layer | Công nghệ |
|---|---|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 + SQL Server (Azure) |
| Auth | JWT Bearer (Access + Refresh tokens) |
| Pattern | Clean Architecture + CQRS (MediatR) + Repository |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Docs | Swagger / OpenAPI |
| Host | IIS inprocess (AspNetCoreModuleV2) |
| File storage | Local disk (`uploads/image/`, `uploads/video/`, ...) |
| Coupon | Coupon entity + CouponUsage (percent & fixed discount) |
| Wallet | UserWallet + WalletTransaction (ledger pattern) |
| Loyalty | UserLoyalty + PointTransaction + LoyaltyTier |
| Dashboard | Aggregate queries theo tháng / quý / category |

---

## Quick Start (Local)

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB hoặc Azure)
- Visual Studio 2022 / VS Code

### 1. Clone + cấu hình

```bash
git clone https://github.com/minhnhatluongg/E-LaptopShop.git
cd E-LaptopShop
```

Tạo file `E-LaptopShop/appsettings.Local.json` (gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ELaptopShopDb;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-32-char-secret-key-here!!!",
    "Issuer": "E-LaptopShop-API",
    "Audience": "E-LaptopShop-Client",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true,
    "ClockSkewMinutes": 5,
    "RequireHttpsMetadata": false,
    "SaveToken": false
  }
}
```

### 2. Apply migrations + seed

```bash
# Apply EF migrations
dotnet ef database update --project E-LaptopShop.Infra --startup-project E-LaptopShop

# Run ứng dụng (DbInitializer tự seed 2 user: admin + customer)
dotnet run --project E-LaptopShop
```

### 3. Seed dữ liệu sản phẩm (chạy SQL trên DB)

```
Database/09_seed_data.sql         ← 1303 sản phẩm từ Kaggle (brands, categories, products)
Database/10_seed_products_2026.sql ← 50 sản phẩm mới nhất 2025-2026 (giá thực tế)
Database/11_seed_manual_23products.sql ← 23 sản phẩm flagship đã chọn lọc
Database/12_migration_wallet.sql  ← Tạo bảng UserWallets + WalletTransactions
```

Thứ tự chạy: `09` → `10` → `11` → `12`

### 4. Test accounts

| Email | Password | Role |
|---|---|---|
| `admin@elaptopshop.com` | `Test@123` | Admin |
| `customer@elaptopshop.com` | `Test@123` | Customer |

---

## API Endpoints

Xem đầy đủ tại Swagger UI. Tóm tắt:

### Public (không cần token)
```
GET  /api/v1/brands/active
GET  /api/v1/categories
GET  /api/v1/products/GetAllProducts
GET  /api/v1/products/GetProductById/{id}
GET  /api/v1/productimage/GetByProductId?productId={id}
GET  /api/v1/coupons/code/{code}
POST /api/v1/auth/login
POST /api/v1/auth/register
POST /api/v1/auth/refresh-token
GET  /api/v1/banners/active
```

### Customer (JWT required)
```
GET  /api/v1/auth/me
GET  /api/v1/auth/me/profile
PUT  /api/v1/auth/me/profile
PUT  /api/v1/auth/me/password
PUT  /api/v1/auth/me/avatar
POST /api/v1/auth/logout

GET  /api/v1/shoppingcart
POST /api/v1/shoppingcart/items
PUT  /api/v1/shoppingcart/items/{id}
DELETE /api/v1/shoppingcart/items/{id}

POST /api/v1/orders
GET  /api/v1/orders/my-orders

GET  /api/v1/wallet/me
GET  /api/v1/wallet/me/transactions

POST /api/v1/coupons/apply
```

### Admin (Admin/Manager role)
```
GET  /api/v1/dashboard/overview
GET  /api/v1/dashboard/chart/revenue
GET  /api/v1/dashboard/chart/orders
GET  /api/v1/dashboard/chart/users
GET  /api/v1/dashboard/chart/top-products

POST /api/v1/products/CreateProduct
PUT  /api/v1/products/UpdateProduct/{id}
DELETE /api/v1/products/DeleteProduct/{id}

POST /api/v1/banners
PUT  /api/v1/banners/{id}
DELETE /api/v1/banners/{id}
PUT  /api/v1/banners/{id}/order

POST /api/v1/wallet/topup
POST /api/v1/wallet/adjust
PUT  /api/v1/wallet/user/{userId}/lock

POST /api/v1/coupons
PUT  /api/v1/coupons/{id}
DELETE /api/v1/coupons/{id}

GET  /api/v1/users/GetPagedUsers
PUT  /api/v1/users/UpdateUser/{id}
```

---

## Database — thư mục `Database/`

```
Database/
├── 00_ERD_diagram.md                   ← Entity Relationship Diagram
├── 01_schema_current.sql               ← Schema hiện tại (toàn bộ tables)
├── 02_schema_upgrade.sql               ← Upgrade scripts
├── 03_data_migration_template.sql      ← Migration template
├── 04_AZURE_SETUP_GUIDE.md             ← Hướng dẫn setup Azure SQL
├── 05_ARCHITECTURE_REVIEW.md           ← Architecture review
├── 06_EF_WORKFLOW_GUIDE.md             ← EF Core workflow guide
├── 07_SCAFFOLD_POST_PROCESSING_EXAMPLE.md
├── 08_IIS_DEPLOY_TROUBLESHOOTING.md    ← IIS deploy troubleshooting
├── 09_seed_data.sql                    ← Kaggle 1303 products (brands/categories)
├── 10_seed_products_2026.sql           ← 50 sản phẩm mới 2025-2026
├── 11_seed_manual_23products.sql       ← 23 flagship products (chọn lọc)
└── 12_migration_wallet.sql             ← Tạo UserWallets + WalletTransactions
```

### Brands được cover trong `10_seed_products_2026.sql`
Apple · Microsoft · Razer · LG · Samsung · MSI · Huawei · Xiaomi · Dell · HP · Lenovo · ASUS · Acer · Google · Fujitsu

### Categories (từ `09_seed_data.sql`)
Ultrabook · Notebook · Netbook · Gaming · 2-in-1 Convertible · Workstation

---

## File Storage

Files upload lưu trên local disk của server:

```
{ContentRoot}/
├── uploads/
│   ├── image/
│   │   ├── avatars/        ← avatar user
│   │   ├── banners/        ← banner carousel
│   │   └── products/{id}/  ← ảnh theo sản phẩm
│   ├── video/
│   ├── document/
│   └── other/
└── temp-uploads/           ← chunks tạm trong khi upload
```

Static file serving:
- `/image/...` → `uploads/image/...`
- `/uploads/...` → `uploads/...`

Upload flow: `POST /file/upload-file?context=products/23` → `{ sysFileId, fileUrl }` → `POST /productimage/product/23/images { sysFileId }`

---

## Deployment (IIS)

### IIS Sites (server `10.10.212.1`)
| Site | Domain | Path |
|---|---|---|
| BE API | `be-shopminhnhat.win-tech.vn` | `D:\IIS WEB\be-shopminhnhat.win-tech.vn` |
| FE | `be-shopminhnhat.click` | `D:\IIS WEB\be-shopminhnhat.click` |

### Publish BE
```bash
dotnet publish E-LaptopShop -c Release -o ./publish
# Copy publish/* → D:\IIS WEB\be-shopminhnhat.win-tech.vn\
# Restart App Pool
```

### Publish FE
```bash
cd ../fe-LaptopShop/app
npm run build
# Copy dist/* → D:\IIS WEB\be-shopminhnhat.click\
```

### IIS App Pool settings
- **BE**: No Managed Code, ASPNETCORE_ENVIRONMENT=Production, inprocess hosting
- **FE**: No Managed Code, URL Rewrite Module required

---

## Docs — thư mục `docs/`

```
docs/
└── SETUP_GUIDE.md    ← Chi tiết setup local + Azure + IIS
```

---

## Environment Files

| File | Dùng khi |
|---|---|
| `appsettings.json` | Defaults (không chứa secret) |
| `appsettings.Development.json` | Local dev (Azure SQL + short token) |
| `appsettings.Production.json` | Production (IIS) |
| `appsettings.Local.json` | Override local (gitignored) |

---

## Tính năng đã implement

- [x] Auth: Login / Register / Refresh Token / Logout / Me
- [x] Profile: GET/PUT /me/profile, PUT /me/password, PUT /me/avatar
- [x] Products: CQRS full (Create/Update/Delete/GetAll/GetById + filter)
- [x] Product Images: Upload (2-step: file → link), SetMain, Delete
- [x] Product Specifications: CRUD
- [x] Brands / Categories: CRUD
- [x] Shopping Cart: Add/Update/Remove/Clear
- [x] Orders: Customer (Create/MyOrders/Cancel) + Admin (All/UpdateStatus)
- [x] User Addresses: CRUD + SetDefault
- [x] Coupon: CRUD + Validate + Redeem
- [x] Wallet: GetBalance + Transactions + Admin TopUp/Adjust/Lock
- [x] Banner: CRUD + GetActive + SetOrder (cho Homepage carousel)
- [x] Dashboard: Overview + Flexible chart API (revenue/orders/users/top-products) với filter theo ngày + category
- [x] Inventory History: Read (Admin)
- [x] File Upload: Chunked upload + single-file + context-based subfolder

## Roadmap

- [ ] Email verification (SMTP)
- [ ] GiftCard entity + redeem flow
- [ ] Loyalty API (UserLoyalty + PointTransaction)  
- [ ] Order integrate with Wallet (pay / refund)
- [ ] Product reviews + rating
- [ ] SignalR notifications

---

**Made with ❤️ by Minh Nhat Luong** · [GitHub](https://github.com/minhnhatluongg/E-LaptopShop)

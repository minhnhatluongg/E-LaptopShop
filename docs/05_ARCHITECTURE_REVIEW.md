# Đánh giá kiến trúc — E-LaptopShop

> Review combo **Clean Architecture + Repository + CQRS + MediatR**.
> Mục tiêu: chuẩn hoá flow, phát hiện anti-patterns, đề xuất cải tiến để mạnh CV/portfolio.

---

## 1. Tổng quan structure hiện tại

```
E-LaptopShop.sln
├── E-LaptopShop                  ← Presentation (API)
│   ├── Controllers/              ← Thin controllers, gọi MediatR
│   ├── Helpers/
│   ├── Properties/
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Example.json
│
├── E-LaptopShop.Application      ← Use cases (Application layer)
│   ├── Common/ (Behaviors, Exceptions, Helpers, Pagination, Queries)
│   ├── DTOs/
│   ├── Features/                 ← CQRS: Commands + Queries + Validations
│   │   ├── Brands/
│   │   ├── Categories/
│   │   ├── Products/
│   │   ├── Orders/
│   │   ├── Inventory/
│   │   ├── ShoppingCart/
│   │   ├── ProductImage/
│   │   ├── SysFile/
│   │   ├── User/
│   │   ├── UserAddress/
│   │   ├── Roles/
│   │   └── Auth/
│   ├── Mappings/                 ← AutoMapper Profiles
│   ├── Services/
│   │   ├── Base/                 ← BaseService<T>
│   │   ├── Interfaces/
│   │   └── Implementations/
│   └── Models/                   ← ApiResponse wrapper
│
├── E-LaptopShop.Domain           ← Core / Entities (innermost ring)
│   ├── Entities/
│   ├── Enums/
│   ├── FilterParams/
│   ├── Repositories/             ← Interfaces (Domain port)
│   │   └── Base/IBaseRepository<T>
│   ├── Sort_Search_Options/
│   └── ValueObjects/
│
└── E-LaptopShop.Infra            ← Infrastructure
    ├── Common/                   (RoleLookup, SlugGenerator, PasswordHasher)
    ├── Data/ApplicationDbContext.cs
    ├── Repositories/             (EF implementations + BaseRepository)
    └── ConfigureServices.cs
```

**Dependency direction** (theo Clean Architecture):

```
Presentation  →  Application  →  Domain  ←  Infrastructure
                                    ▲
                                    └────── Application
```

Domain là vòng trong nhất, không reference ai. Application reference Domain. Infrastructure reference Domain (để implement repository interfaces) + Application. Presentation reference Application.

→ **Hiện tại structure ĐÚNG hướng dependency**. Tốt 👍

---

## 2. Chấm điểm từng pattern

### 2.1 Clean Architecture — 7.5/10

**Đúng:**
- 4 layer rõ ràng (Presentation / Application / Domain / Infra).
- Domain không có dependency vào framework (chỉ DataAnnotations, EF attributes — vẫn chấp nhận được cho project học tập).
- Interface ở Domain, implementation ở Infra → Dependency Inversion.

**Sai / cần cải thiện:**

| # | Vấn đề | Vị trí | Fix |
| --- | --- | --- | --- |
| 1 | `ApplicationDbContext` đặt trong namespace `E_LaptopShop.Domain.Entities` — DbContext không phải Domain | `E-LaptopShop.Infra/Data/ApplicationDbContext.cs` line 5 | Đổi namespace thành `E_LaptopShop.Infra.Data` |
| 2 | Domain entities có EF-specific attributes (`[Index]`, `[InverseProperty]`) | Hầu hết entities | Chấp nhận được cho project nhỏ. Pro hơn: dùng Fluent API trong DbContext, giữ entity "POCO" thuần |
| 3 | Có nhiều file `*Example.cs` và `[Obsolete]` còn trong project (vd `ProductImageRepositoryExample.cs`, `CreateProductImageDto`, `Legacy Methods`) | Nhiều | Xoá hẳn hoặc move sang folder `docs/` — code production không nên có "Example" |
| 4 | `appsettings.Example.json` đặt trong project — không có ý nghĩa, ASP.NET không load | `E-LaptopShop/appsettings.Example.json` | Đổi thành `appsettings.Sample.json` hoặc `appsettings.template.json` và viết README chỉ rõ đây là mẫu |

### 2.2 Repository Pattern — 6/10

**Đúng:**
- Có `IBaseRepository<TEntity, TKey>` generic + base class.
- Interface ở Domain, implementation ở Infra.

**Sai / cần cải thiện:**

| # | Vấn đề | Mức |
| --- | --- | --- |
| 1 | **Repository return `IQueryable<T>` ra ngoài** (vd `ProductImageRepository.GetFilteredQueryable`) — leak EF Core ra Application layer, vi phạm separation | 🔴 Cao |
| 2 | Một số repo (`SysFileRepository`) không kế thừa `BaseRepository<T>` → trùng code, không thống nhất | 🟡 TB |
| 3 | `IBrandRepositoy` typo trong tên (đáng lẽ `IBrandRepository`) | 🟢 Thấp |
| 4 | Method `DeleteAsync` nơi trả về `bool`, nơi trả về `int`, nơi trả về `Task` — không nhất quán | 🟡 TB |
| 5 | Một số repo có `ApplyFilter` bị comment-out (vd `ProductImageRepository.GetAllFilterAndPagination` line 149) | 🟡 TB |
| 6 | Có cả `ProductImageRepository` VÀ `ProductImageRepositoryExample` cùng implement `IProductImageRepository` → ai inject? Hiện tại `ConfigureServices` đăng ký `ProductImageRepository`, file `Example` thành dead code | 🟡 TB |
| 7 | Một số method trả `null` ngầm, một số throw `KeyNotFoundException` → contract không nhất quán | 🟡 TB |

**Khuyến nghị**:
- Quyết định: hoặc `GetByIdAsync` trả `T?` (null nếu không có), hoặc throw. **Pick ONE và áp dụng toàn project**.
- Repository không bao giờ trả `IQueryable` ra ngoài → trả `IReadOnlyList<T>` hoặc dùng Specification pattern.

### 2.3 CQRS với MediatR — 8/10

**Đúng:**
- Folder `Features/` theo vertical slice (mỗi feature 1 folder, có Commands/Queries/Validations).
- Dùng `IRequest<TResponse>` + `IRequestHandler<,>` của MediatR đúng cách.
- Controller mỏng, chỉ gọi `_mediator.Send(...)`.
- Có Behaviors folder (logging, validation pipeline).

**Sai / cần cải thiện:**

| # | Vấn đề | Cụ thể |
| --- | --- | --- |
| 1 | Một số handler trả `int` để báo "rows affected" hoặc "deleted id" — không idiomatic | `DeleteUserCommandHandler`, `DeleteRoleCommandHandler`, `DeleteUserAddressCommandHandler` (đã fix) |
| 2 | Một số handler đang gọi thẳng `IRepository` (vd `DeleteUserCommand` gọi `IUserRepository.DeleteAsync`) — bỏ qua Service layer, không nhất quán với pattern khác (vd `DeleteBrandCommandHandler` gọi `IBrandService`) | Nhiều handler |
| 3 | Naming: file `hardDeleteUserAddressCommandHandler.cs` viết chữ thường — vi phạm convention C# PascalCase | `Features/UserAddress/Commands/HardDeleteUserAddress/` |
| 4 | `Categories/Queries/GetCategoryById/GetCategoryByIdQueryHandler` không có logger — không nhất quán | Code style |
| 5 | Có thư mục `Validations` riêng nhưng nhiều Command không có Validator | Chỉ vài entity có |

**Khuyến nghị**:
- Chọn 1 trong 2 đường: handler gọi Service, hoặc handler gọi Repository — đừng trộn. **Khuyến nghị: handler gọi Service** (vì service đã đóng gói business rule + validation).
- Tách rõ Command vs Query: Command return ít data (chỉ `int Id` hoặc `Result`), Query return DTO.
- Mọi Command nên có Validator (FluentValidation) đăng ký qua MediatR pipeline behavior.

### 2.4 Service layer (Bonus) — 7/10

**Có `BaseService<TEntity, TDto, TCreateDto, TUpdateDto, TQueryParams>`** — generic mạnh, đẹp.

**Sai:**
- Service trộn cả CRUD lẫn business operation. Tốt cho project nhỏ, nhưng khi to ra nên tách `IXxxQueryService` + `IXxxCommandService`.
- `ProductImageService` có "Legacy Methods" với `[Obsolete]` → dọn dẹp.

### 2.5 Cấu hình & Secrets — 9/10 (sau khi đã setup User Secrets)

- ✅ Đã setup User Secrets cho dev local.
- ✅ Có `appsettings.Example.json` làm template.
- ⚠️ Cần đảm bảo `appsettings.json` (file commit) **KHÔNG chứa connection string thật** — chỉ có template rỗng `""`.
- ⚠️ Khi deploy lên production, dùng **Azure Key Vault + Managed Identity** thay vì env var có chứa password.

### 2.6 Test — 0/10

**Project chưa có test project nào!** Đây là **điểm trừ lớn nhất** cho CV. Recruiter senior sẽ check ngay.

→ Khuyến nghị: tạo thêm **2 test projects**:

```
E-LaptopShop.Application.UnitTests/   ← test Handlers, Services (mock repositories)
E-LaptopShop.IntegrationTests/        ← test Controllers + EF (in-memory SQLite hoặc Testcontainers)
```

Cần **tối thiểu 1 unit test** + **1 integration test** chạy được. Còn hơn 0.

---

## 3. Flow chuẩn (best-practice template)

Ví dụ: `POST /api/brands` — tạo brand mới.

```
HTTP Request
     │
     ▼
[BrandController.Create]   ◄── Presentation
     │   await _mediator.Send(new CreateBrandCommand(dto))
     ▼
[MediatR Pipeline]
     │   ValidationBehavior  ◄── FluentValidation
     │   LoggingBehavior
     │   TransactionBehavior (optional)
     ▼
[CreateBrandCommandHandler]   ◄── Application
     │   await _brandService.CreateAsync(...)
     ▼
[BrandService.CreateAsync]    ◄── Application
     │   ValidateCreateDto()        ← business rule
     │   _mapper.Map<Brand>(dto)
     │   await _brandRepository.AddAsync(entity)
     ▼
[BrandRepository.AddAsync]    ◄── Infrastructure
     │   _context.Brands.Add(entity)
     │   await _context.SaveChangesAsync()
     ▼
[ApplicationDbContext / EF Core]
     │   INSERT INTO Brands ...
     ▼
[SQL Server / Azure SQL]
```

**Project bạn ĐÃ implement đúng flow này cho `Brand`**. Vài entity khác bỏ Service layer, gọi thẳng Repository — không chuẩn, nên đồng bộ lại.

---

## 4. Top 10 việc nên làm tiếp theo (theo priority)

| # | Việc | Lý do | Effort |
| --- | --- | --- | --- |
| 1 | Đổi password Azure SQL đã leak + xoá khỏi `03_data_migration_template.sql` | Security khẩn cấp | 5 phút |
| 2 | Thêm test project + 5-10 unit test cơ bản | CV killer feature | 1-2 ngày |
| 3 | Đồng bộ: handler chỉ gọi Service, không gọi Repository | Tránh hỗn loạn pattern | 1 ngày |
| 4 | Xoá hết `*Example*.cs` và `[Obsolete]` methods | Code sạch | 1 giờ |
| 5 | Setup CI/CD GitHub Actions: build + test khi push | Show DevOps skill | 2 giờ |
| 6 | Thêm Swagger annotation đầy đủ + 1 API key sample | UX cho recruiter test API | 2 giờ |
| 7 | Triển khai `Result<T>` pattern thay vì throw exception | Error handling pro | 1 ngày |
| 8 | Thêm Specification pattern thay cho `GetFilteredQueryable` | Hide EF khỏi Application | 1 ngày |
| 9 | Thêm Domain Events + MediatR `INotification` | Show DDD knowledge | 1-2 ngày |
| 10 | Thêm Background Job (Hangfire/Quartz) cho 1 task: gửi email confirm | Show full-stack thinking | 1 ngày |

---

## 5. Những bug đã fix trong lần review này

| File | Bug | Fix |
| --- | --- | --- |
| `CreateBrandCommandHandler.cs` line 20 | `brandService = _brandService;` (gán ngược → field luôn null) | Đảo lại: `_brandService = brandService;` |
| `ISysFileRepository.cs` | Kế thừa `IBaseRepository<SysFile>` nhưng implementation không match → mọi call site break | Tách interface độc lập, chỉ giữ methods thực sự dùng (Interface Segregation) |
| `SysFileRepository.AddAsync` | Trả `int` thay vì `SysFile` → interface base mismatch | Đổi trả `SysFile`, sửa 2 call sites lấy `.Id` |
| `SysFileRepository.GetByIdAsync` | Trả `SysFile` non-null + throw nếu null → contract khác interface base | Đổi trả `SysFile?` (null nếu không có), validation chuyển lên service |
| `DeleteUserCommandHandler`, `DeleteRoleCommandHandler`, `DeleteUserAddressCommandHandler`, `DeleteProductSpecificationCommandHandler` | Trả về `Task<int>` nhưng `Repository.DeleteAsync` trả `Task<bool>` → compile error | Wrap: `return ok ? request.Id : 0;` |
| `GetCategoryByIdQueryHandler`, `GetProductByIdQueryHandler` | Method trả `Task<TDto?>` nhưng interface `IRequestHandler<,TDto>` non-null → warning nullability | Đổi method trả `Task<TDto>` và `throw KeyNotFoundException` nếu null |
| `.gitignore` | Ignore tất cả `appsettings.*.json` kể cả file gốc; thiếu secret patterns | Restructure: chỉ ignore `Development`/`Production`/`Local`, thêm patterns cho `.env`, `*.pfx`, `*.key`, IDE folders, OS files, local DB |

---

## 6. Note về Performance / Production-readiness

- ✅ DbContext lifetime `Scoped` — đúng.
- ✅ EF `AsNoTracking()` dùng đúng trong query-only.
- ⚠️ Chưa thấy `EnableRetryOnFailure` trong DbContext — bắt buộc cho Azure SQL Serverless.
- ⚠️ Chưa thấy connection pool tuning.
- ⚠️ Chưa thấy distributed cache (Redis) cho hot data (Products list, Categories tree).
- ⚠️ Logging: dùng `ILogger<T>` đúng, nhưng chưa thấy structured logging output → đề xuất Serilog + Application Insights khi deploy.

---

**Kết luận**: Project đang ở mức **6.5/10** so với "production-ready". Sau khi làm xong Top 10 việc bên trên có thể lên **8.5/10** — đủ mạnh để khoe trong portfolio.

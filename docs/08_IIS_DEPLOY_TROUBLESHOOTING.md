# Troubleshooting IIS — 404 toàn cục khi publish

> Hiện tượng: publish lên IIS, mọi URL (kể cả `/`) đều trả 404. Không có log.
> **Nguyên nhân thường gặp** (theo xác suất giảm dần):

---

## A. App KHÔNG start được (90% case)

IIS không chạy được app → fallback default 404. Cần enable logging trước để biết lỗi gì.

### Bước 1 — Enable stdout log trong web.config

Mở `web.config` trên server (tại deploy folder), đổi:

```xml
<aspNetCore processPath=".\E-LaptopShop.exe"
            stdoutLogEnabled="true"          <!-- ✅ đổi từ false -->
            stdoutLogFile=".\logs\stdout"
            hostingModel="inprocess">
```

Tạo folder `logs/` và cấp quyền:

```powershell
cd C:\inetpub\wwwroot\E-LaptopShop   # hoặc deploy path của bạn
mkdir logs
icacls logs /grant "IIS_IUSRS:(OI)(CI)F"
icacls logs /grant "IIS AppPool\<AppPoolName>:(OI)(CI)F"
```

Replace `<AppPoolName>` với tên app pool thật (xem trong IIS Manager → Application Pools).

### Bước 2 — Restart IIS, gọi URL, đọc log

```powershell
iisreset
# Hoặc chỉ restart app pool:
# Restart-WebAppPool -Name "<AppPoolName>"

# Gọi URL để trigger startup
curl http://localhost  # hoặc URL của site

# Đọc log mới nhất
Get-ChildItem C:\inetpub\wwwroot\E-LaptopShop\logs\*.log |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 |
    Get-Content
```

→ Sẽ thấy stack trace cụ thể của lỗi crash. Paste cho tôi để fix.

### Lỗi crash thường gặp khi startup .NET 9

| Lỗi trong log | Nguyên nhân | Fix |
| --- | --- | --- |
| `Could not load file or assembly 'Microsoft.AspNetCore.App'` | Chưa cài ASP.NET Core Runtime 9 trên server | Cài [ASP.NET Core 9.0 Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) |
| `Connection string 'DefaultConnection' was not found` | `appsettings.json` không có/sai | Set qua env var `ConnectionStrings__DefaultConnection` |
| `JWT Settings not found in configuration` | Thiếu section `JwtSettings` | Copy section này vào `appsettings.json` server |
| `KeyNotFoundException` khi seed | DB chưa migrate / connection sai | Fix connection hoặc tạm comment dòng `DbInitializer.SeedAsync` |
| `Cannot open server '...' requested by the login` | Firewall Azure chặn IP server | Add IP server vào Azure SQL firewall |
| `Login failed for user 'admin_laptopshop'` | Sai password | Update env var `ConnectionStrings__DefaultConnection` |
| `Could not find file '...\E-LaptopShop.xml'` | Publish thiếu XML doc | Build with `/p:GenerateDocumentationFile=true` hoặc bọc try/catch quanh Swagger XML |

---

## B. Thiếu ASP.NET Core Hosting Bundle

### Check

```powershell
# Trên server
dotnet --list-runtimes
```

Phải thấy:
```
Microsoft.AspNetCore.App 9.0.x
Microsoft.NETCore.App 9.0.x
```

Và:

```powershell
Test-Path "$env:ProgramFiles\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
```

Phải trả `True`.

### Fix

Tải [ASP.NET Core Hosting Bundle 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) → mục **"Hosting Bundle"** → cài. Sau đó:

```powershell
net stop was /y
net start w3svc
# Hoặc: iisreset
```

---

## C. web.config thiếu hoặc sai

### Check

Trong deploy folder phải có `web.config`. Nếu thiếu → 404 toàn cục.

### Mẫu web.config chuẩn cho .NET 9 in-process

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath=".\E-LaptopShop.exe"
                  stdoutLogEnabled="true"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
      <!-- Tăng giới hạn upload nếu cần -->
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
    </system.webServer>
  </location>
</configuration>
```

---

## D. Sai cách publish

### KHÔNG làm thế này

- Copy bin/Debug/ hoặc bin/Release/ thủ công → thiếu nhiều file
- Build mà không publish

### LÀM thế này

```powershell
cd D:\project-building-CV\laptop_shop\E-LaptopShop\E-LaptopShop

# Publish framework-dependent (cần Hosting Bundle trên server)
dotnet publish -c Release `
  -r win-x64 `
  --self-contained false `
  -o C:\Deploy\E-LaptopShop

# HOẶC publish self-contained (KHÔNG cần Hosting Bundle, nặng hơn ~80MB)
dotnet publish -c Release `
  -r win-x64 `
  --self-contained true `
  -o C:\Deploy\E-LaptopShop
```

Sau đó copy nguyên folder `C:\Deploy\E-LaptopShop\` sang server.

### Hoặc dùng Visual Studio

- Right-click project `E-LaptopShop` → **Publish...**
- Target: **Folder**
- Folder path: chọn folder local hoặc UNC tới server
- Click Publish

VS sẽ tự tạo `web.config`, copy đủ DLL, runtime config.

---

## E. Config Production thiếu (rất hay gặp)

App dev dùng **User Secrets** — server KHÔNG có. Phải set ở chỗ khác.

### Option 1: appsettings.Production.json (đơn giản)

Tạo trên server tại deploy folder:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:be-laptopshop.database.windows.net,1433;Initial Catalog=free-sql-db-9719142;User ID=admin_laptopshop;Password=<password>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"
  },
  "JwtSettings": {
    "SecretKey": "<strong-random-key-min-32-chars>",
    "Issuer": "E-LaptopShop-API",
    "Audience": "E-LaptopShop-Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true,
    "ClockSkewMinutes": 5,
    "RequireHttpsMetadata": true,
    "SaveToken": false
  }
}
```

⚠️ Đảm bảo `ASPNETCORE_ENVIRONMENT = Production` (set trong `web.config` như mẫu trên).

### Option 2: Environment Variables (chuẩn enterprise)

Trong IIS Manager:
1. Chọn site
2. Configuration Editor (icon góc phải)
3. Section: `system.webServer/aspNetCore`
4. Click `environmentVariables` → `(Collection)` → `...`
5. Add các entry:

```
ASPNETCORE_ENVIRONMENT = Production
ConnectionStrings__DefaultConnection = Server=tcp:...
JwtSettings__SecretKey = <secret>
JwtSettings__Issuer = E-LaptopShop-API
JwtSettings__Audience = E-LaptopShop-Client
```

⚠️ Dùng `__` (2 dấu gạch dưới) thay cho `:` trong tên env var.

Save → Restart app pool.

---

## F. Permission issue

App Pool identity (mặc định `IIS AppPool\<AppPoolName>`) **không có quyền** đọc/ghi:

```powershell
# Cấp quyền read+execute cho deploy folder
icacls C:\inetpub\wwwroot\E-LaptopShop /grant "IIS AppPool\<AppPoolName>:(OI)(CI)RX"

# Cấp full quyền cho logs + uploads (vì app cần write)
icacls C:\inetpub\wwwroot\E-LaptopShop\logs /grant "IIS AppPool\<AppPoolName>:(OI)(CI)F"
icacls C:\inetpub\wwwroot\E-LaptopShop\uploads /grant "IIS AppPool\<AppPoolName>:(OI)(CI)F"
icacls C:\inetpub\wwwroot\E-LaptopShop\temp-uploads /grant "IIS AppPool\<AppPoolName>:(OI)(CI)F"
icacls C:\inetpub\wwwroot\E-LaptopShop\wwwroot /grant "IIS AppPool\<AppPoolName>:(OI)(CI)F"
```

---

## G. Site binding sai

Trong IIS Manager → site → Bindings: kiểm tra
- Port (80? 5000?)
- IP (`*` = mọi IP, hay specific?)
- Host name

URL bạn gọi phải khớp với binding.

---

## Quy trình debug 5 phút

1. **Trên server** — check ANCM + .NET 9 runtime:
   ```powershell
   dotnet --list-runtimes
   Test-Path "$env:ProgramFiles\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
   ```
   → Nếu thiếu → cài Hosting Bundle, restart IIS, test lại.

2. **Check deploy folder**:
   ```powershell
   cd C:\inetpub\wwwroot\E-LaptopShop
   dir | Where-Object { $_.Name -in 'web.config','E-LaptopShop.dll','E-LaptopShop.exe','appsettings.json' }
   ```
   → Phải có 4 file. Thiếu → publish lại đúng cách.

3. **Bật stdout log** trong `web.config` → restart app pool → gọi URL → đọc log.

4. **Paste log error** cho tôi.

---

## Smoke test endpoint

Sau khi tôi đã thêm 2 endpoint vào `Program.cs`:

```
GET /         → JSON { name, status, environment, time }
GET /health   → "Healthy"
```

Khi deploy lên server, gọi `http://<server>/health`:
- Trả `"Healthy"` → app start OK, vấn đề ở routing/config khác.
- Trả 404 → app **KHÔNG start được** → quay lại bước 1-3 ở trên.
- Trả 500 → app start nhưng request fail → check stdout log.

---

## Checklist 1 trang

- [ ] Hosting Bundle 9.0 cài trên server
- [ ] Deploy bằng `dotnet publish` (không phải copy bin/)
- [ ] `web.config` tồn tại trong deploy folder
- [ ] `web.config` có `stdoutLogEnabled="true"`
- [ ] Folder `logs/` tồn tại, có quyền write cho IIS AppPool
- [ ] App Pool dùng "**No Managed Code**" (vì .NET Core không phải .NET Framework)
- [ ] App Pool identity có quyền đọc deploy folder
- [ ] `ASPNETCORE_ENVIRONMENT=Production` set qua web.config hoặc env var
- [ ] `appsettings.Production.json` hoặc env var có `ConnectionStrings:DefaultConnection`
- [ ] `JwtSettings` có đủ trong config Production
- [ ] Azure SQL firewall đã add IP của server
- [ ] Site binding match URL bạn gọi
- [ ] Test `GET /health` trả "Healthy"

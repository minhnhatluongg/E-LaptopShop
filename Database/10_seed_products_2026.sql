/* =============================================================
   E-LaptopShop — Seed: 50 sản phẩm 2025-2026 (giá mới nhất)
   Covers: Microsoft, Razer, LG, Samsung, MSI, Huawei, Xiaomi,
           Apple, Dell, HP, Lenovo, ASUS, Acer (đầy đủ brands)
   Idempotent: dùng slug để tránh duplicate.
   Run AFTER 09_seed_data.sql (brands/categories phải tồn tại)
   ============================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ============================================================
-- 1. Đảm bảo brands còn thiếu tồn tại
-- ============================================================
-- Razer đã có Id=14, LG Id=19, Samsung Id=16, MSI Id=8,
-- Microsoft Id=9, Huawei Id=11, Xiaomi Id=12 (từ 09_seed_data.sql)
-- Chỉ cần confirm slug để INSERT sản phẩm đúng BrandId

-- ============================================================
-- 2. Đảm bảo categories tồn tại (từ 09_seed_data.sql):
--    1=Ultrabook, 2=Notebook, 3=Netbook, 4=Gaming,
--    5=2 in 1 Convertible, 6=Workstation
-- ============================================================

-- ============================================================
-- 3. INSERT 50 sản phẩm (WHERE NOT EXISTS theo slug)
-- ============================================================

-- Helper: resolve BrandId và CategoryId bằng slug/name an toàn

-- ─── APPLE MacBook (4 sản phẩm) ──────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Apple MacBook Pro 14" M4 Pro', N'apple-macbook-pro-14-m4-pro',
    N'Chip Apple M4 Pro 12-nhân CPU 20-nhân GPU, 24GB Unified Memory, SSD 512GB, 14.2" Liquid Retina XDR 3024x1964 120Hz ProMotion, pin 24 giờ. Hiệu suất đỉnh cho developer và creative pro.',
    59990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='apple'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'apple-macbook-pro-14-m4-pro');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Apple MacBook Pro 16" M4 Max', N'apple-macbook-pro-16-m4-max',
    N'Chip Apple M4 Max 16-nhân CPU 40-nhân GPU, 48GB Unified Memory, SSD 1TB, 16.2" Liquid Retina XDR 3456x2234 120Hz, pin 24 giờ. Render 8K và AI không giới hạn.',
    99990000.00, 0.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='workstation'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='apple'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'apple-macbook-pro-16-m4-max');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Apple MacBook Air 13" M4', N'apple-macbook-air-13-m4',
    N'Chip Apple M4 8-nhân CPU 10-nhân GPU, 16GB Unified Memory, SSD 256GB, 13.6" Liquid Retina 2560x1664, fanless hoàn toàn, 1.24kg, pin 18 giờ. Bestseller 2025.',
    29990000.00, 8.00, 30,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='apple'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'apple-macbook-air-13-m4');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Apple MacBook Air 15" M4', N'apple-macbook-air-15-m4',
    N'Chip Apple M4 8-nhân CPU 10-nhân GPU, 16GB Unified Memory, SSD 512GB, 15.3" Liquid Retina 2880x1864, 1.51kg, pin 18 giờ. Màn hình lớn nhất dòng Air.',
    36990000.00, 5.00, 20,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='apple'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'apple-macbook-air-15-m4');

-- ─── MICROSOFT Surface (5 sản phẩm) ──────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Microsoft Surface Laptop 7 13"', N'microsoft-surface-laptop-7-13',
    N'Snapdragon X Elite X1E-80-100 (12-nhân, 3.4GHz), 16GB LPDDR5, SSD 512GB, 13.8" PixelSense Flow 2304x1536 120Hz HDR touch, Copilot+PC, pin 22 giờ.',
    34990000.00, 5.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='microsoft'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'microsoft-surface-laptop-7-13');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Microsoft Surface Laptop 7 15"', N'microsoft-surface-laptop-7-15',
    N'Snapdragon X Elite X1E-80-100 (12-nhân), 32GB LPDDR5x, SSD 1TB, 15" PixelSense Flow 2496x1664 120Hz HDR, Copilot+ AI features, pin 22 giờ.',
    44990000.00, 0.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='microsoft'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'microsoft-surface-laptop-7-15');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Microsoft Surface Pro 11', N'microsoft-surface-pro-11',
    N'Snapdragon X Plus X1P-64-100, 16GB LPDDR5x, SSD 256GB, 13" PixelSense Flow 2880x1920 120Hz OLED touch, detachable keyboard, bút Surface Slim Pen 2.',
    37990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='microsoft'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'microsoft-surface-pro-11');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Microsoft Surface Book 4', N'microsoft-surface-book-4',
    N'Intel Core Ultra 7 165H, NVIDIA RTX 4060 8GB, 32GB LPDDR5, SSD 512GB, 15" PixelSense 3240x2160 touch detachable. Laptop 2-in-1 cao cấp cho enterprise.',
    69990000.00, 0.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='microsoft'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'microsoft-surface-book-4');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Microsoft Surface Laptop Studio 2', N'microsoft-surface-laptop-studio-2',
    N'Intel Core i7-13700H, NVIDIA RTX 4060 Ada 8GB, 32GB LPDDR5, SSD 512GB, 14.4" PixelSense Flow 2400x1600 120Hz touch, Dynamic Woven Hinge, tặng Surface Slim Pen 2.',
    74990000.00, 5.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='microsoft'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'microsoft-surface-laptop-studio-2');

-- ─── RAZER Blade (4 sản phẩm) ────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Razer Blade 16 2025', N'razer-blade-16-2025',
    N'Intel Core i9-14900HX (24-nhân), NVIDIA RTX 5080 16GB GDDR7, 32GB DDR5 5600MHz, SSD 2TB NVMe PCIe 5.0, 16" QHD+ OLED 2560x1600 240Hz 100% DCI-P3.',
    99990000.00, 0.00, 4,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='razer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'razer-blade-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Razer Blade 14 2025', N'razer-blade-14-2025',
    N'AMD Ryzen 9 8945HX (8-nhân, 5.2GHz), NVIDIA RTX 4070 Ti Super 12GB, 16GB LPDDR5x 6400MHz, SSD 1TB, 14" QHD+ 2560x1600 240Hz 100% DCI-P3, chassis nhôm CNC 17mm.',
    69990000.00, 0.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='razer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'razer-blade-14-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Razer Blade 15 2025', N'razer-blade-15-2025',
    N'Intel Core i9-14900HX, NVIDIA RTX 4080 12GB, 32GB DDR5, SSD 1TB, 15.6" QHD 2560x1440 240Hz 100% DCI-P3, IR webcam, THX Spatial Audio.',
    84990000.00, 0.00, 4,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='razer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'razer-blade-15-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Razer Book 13', N'razer-book-13-2025',
    N'Intel Core Ultra 7 155U (22-nhân), Intel Arc Graphics, 16GB LPDDR5x, SSD 512GB, 13.4" OLED 2880x1800 60Hz 100% DCI-P3 touch, 1.29kg, pin 15 giờ.',
    39990000.00, 5.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='razer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'razer-book-13-2025');

-- ─── LG Gram (4 sản phẩm) ────────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'LG Gram 17 2025', N'lg-gram-17-2025',
    N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz), Intel Arc 140V GPU, 16GB LPDDR5x, SSD 1TB, 17" IPS WQXGA 2560x1600 300 nits, 1.35kg, MIL-STD-810H, pin 25 giờ.',
    39990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lg'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lg-gram-17-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'LG Gram 16 2025', N'lg-gram-16-2025',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 32GB LPDDR5x, SSD 1TB, 16" IPS 2560x1600 350 nits, 1.19kg, 7 chuẩn MIL-STD-810H, Thunderbolt 4 x2.',
    35990000.00, 5.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lg'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lg-gram-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'LG Gram Pro 16 OLED 2025', N'lg-gram-pro-16-oled-2025',
    N'Intel Core Ultra 7 258V, NVIDIA RTX 3050 6GB, 32GB LPDDR5x, SSD 1TB, 16" OLED WQXGA 2560x1600 120Hz HDR 1M:1, 1.34kg, Thunderbolt 4.',
    45990000.00, 0.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lg'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lg-gram-pro-16-oled-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'LG Gram 14 2025', N'lg-gram-14-2025',
    N'Intel Core Ultra 5 226V (8-nhân), Intel Arc 130V, 16GB LPDDR5x, SSD 512GB, 14" IPS 1920x1200 400 nits, 0.99kg — nhẹ nhất thế giới 14".',
    29990000.00, 5.00, 15,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lg'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lg-gram-14-2025');

-- ─── SAMSUNG Galaxy Book (4 sản phẩm) ────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Samsung Galaxy Book5 Pro 360 16"', N'samsung-galaxy-book5-pro-360-16',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 16GB LPDDR5x, SSD 512GB, 16" Dynamic AMOLED 2X 3K 2880x1800 120Hz 400 nits touch, S Pen tích hợp, 1.66kg.',
    44990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='samsung'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'samsung-galaxy-book5-pro-360-16');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Samsung Galaxy Book5 Ultra', N'samsung-galaxy-book5-ultra',
    N'Intel Core Ultra 9 285H (24-nhân), NVIDIA RTX 4070 8GB, 32GB LPDDR5x, SSD 1TB, 16" Dynamic AMOLED 2X 3K 120Hz, Galaxy AI features, 1.86kg.',
    59990000.00, 0.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='workstation'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='samsung'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'samsung-galaxy-book5-ultra');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Samsung Galaxy Book4 Edge', N'samsung-galaxy-book4-edge',
    N'Snapdragon X Elite X1E-84-100 (12-nhân), 16GB LPDDR5x, SSD 512GB, 14" Dynamic AMOLED 2X 2880x1800 120Hz, Copilot+ PC, pin 22 giờ, 1.17kg.',
    34990000.00, 5.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='samsung'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'samsung-galaxy-book4-edge');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Samsung Galaxy Book4 360 15"', N'samsung-galaxy-book4-360-15',
    N'Intel Core Ultra 5 125H, Intel Arc Graphics, 16GB LPDDR5, SSD 512GB, 15.6" FHD AMOLED touch 60Hz, S Pen, hinge 360°, 1.61kg.',
    27990000.00, 5.00, 15,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='samsung'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'samsung-galaxy-book4-360-15');

-- ─── MSI (5 sản phẩm) ────────────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'MSI Titan GT77 2025', N'msi-titan-gt77-2025',
    N'Intel Core i9-14900HX (24-nhân), NVIDIA RTX 5090 24GB GDDR7, 64GB DDR5, SSD 2TB PCIe 5.0, 17.3" UHD IPS 3840x2160 144Hz 100% AdobeRGB. Desktop replacement đỉnh nhất.',
    129990000.00, 0.00, 3,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='msi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'msi-titan-gt77-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'MSI Raider GE78 HX 2025', N'msi-raider-ge78-hx-2025',
    N'Intel Core i9-14900HX, NVIDIA RTX 4090 16GB, 32GB DDR5 4800MHz, SSD 2TB PCIe 5.0, 17" QHD+ 2560x1600 240Hz 100% DCI-P3, MUX Switch, DP 2.1.',
    79990000.00, 5.00, 4,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='msi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'msi-raider-ge78-hx-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'MSI Stealth 16 Studio 2025', N'msi-stealth-16-studio-2025',
    N'Intel Core i9-13980HX (24-nhân), NVIDIA RTX 4080 12GB, 32GB DDR5, SSD 2TB, 16" QHD+ OLED 2560x1600 240Hz 100% DCI-P3, 19mm chassis, tặng Pen.',
    64990000.00, 5.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='msi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'msi-stealth-16-studio-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'MSI Prestige 16 AI Evo 2025', N'msi-prestige-16-ai-evo-2025',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 32GB LPDDR5x, SSD 1TB, 16" IPS+ 2560x1600 120Hz sRGB 100%, 1.65kg, Thunderbolt 4, AI camera, pin 16 giờ.',
    32990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='msi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'msi-prestige-16-ai-evo-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'MSI Vector 17 HX 2025', N'msi-vector-17-hx-2025',
    N'Intel Core i9-14900HX, NVIDIA RTX 4080 12GB, 32GB DDR5 5600MHz, SSD 1TB, 17.3" FHD IPS 360Hz 100% sRGB, MUX Switch. Gaming tốc độ cao extreme.',
    57990000.00, 5.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='msi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'msi-vector-17-hx-2025');

-- ─── HUAWEI (3 sản phẩm) ─────────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Huawei MateBook X Pro 2025', N'huawei-matebook-x-pro-2025',
    N'Intel Core Ultra 9 185H (22-nhân), Intel Arc iGPU, 32GB LPDDR5, SSD 1TB, 14.2" OLED 3120x2080 LTPO 1-120Hz HDR, 1.26kg, Thunderbolt 4. Đẹp nhất phân khúc.',
    44990000.00, 5.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='huawei'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'huawei-matebook-x-pro-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Huawei MateBook 16s 2025', N'huawei-matebook-16s-2025',
    N'Intel Core Ultra 9 185H, Intel Arc iGPU, 32GB LPDDR5, SSD 1TB, 16" IPS+ 2520x1680 60Hz touch, VerMind AI, SuperCharge 90W, 1.99kg.',
    36990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='notebook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='huawei'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'huawei-matebook-16s-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Huawei MateBook D 16 2025', N'huawei-matebook-d-16-2025',
    N'Intel Core i7-13700H, NVIDIA MX550 2GB, 16GB LPDDR4x, SSD 512GB, 16" IPS FHD 1920x1200 300 nits, EyeComfort, 1.68kg, USB-C 65W, SuperSonic Speaker.',
    21990000.00, 8.00, 15,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='notebook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='huawei'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'huawei-matebook-d-16-2025');

-- ─── XIAOMI (3 sản phẩm) ─────────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Xiaomi Mi Notebook Pro X 14 2025', N'xiaomi-mi-notebook-pro-x-14-2025',
    N'Intel Core Ultra 7 155H (22-nhân), NVIDIA RTX 4060 8GB, 32GB LPDDR5, SSD 1TB, 14" OLED 2880x1800 120Hz 100% DCI-P3 touch. Flagship mỏng nhẹ Xiaomi.',
    35990000.00, 8.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='xiaomi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'xiaomi-mi-notebook-pro-x-14-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Xiaomi Book Pro 16 2025', N'xiaomi-book-pro-16-2025',
    N'Intel Core i7-1360P (12-nhân), NVIDIA MX550 2GB, 16GB LPDDR5, SSD 512GB, 16" 3K OLED 3200x2000 120Hz VESA HDR600, 1.8kg, 67W GaN charger.',
    28990000.00, 8.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='notebook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='xiaomi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'xiaomi-book-pro-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Xiaomi Redmi Book 16 2025', N'xiaomi-redmi-book-16-2025',
    N'AMD Ryzen 5 8645HS (6-nhân, 5.0GHz), AMD Radeon 760M iGPU, 16GB LPDDR5, SSD 512GB, 16" FHD IPS 1920x1200 144Hz, 1.58kg, 65W USB-C. Tầm trung cực giá trị.',
    14990000.00, 5.00, 20,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='notebook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='xiaomi'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'xiaomi-redmi-book-16-2025');

-- ─── DELL (4 sản phẩm bổ sung) ───────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Dell XPS 14 2025', N'dell-xps-14-2025',
    N'Intel Core Ultra 7 155H, NVIDIA RTX 4050 6GB, 16GB LPDDR5x, SSD 512GB, 14.5" OLED 3.2K 3200x2000 120Hz VESA HDR 500, touch. Mỏng 16.3mm.',
    52990000.00, 5.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='dell'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'dell-xps-14-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Dell XPS 16 2025', N'dell-xps-16-2025',
    N'Intel Core Ultra 9 185H, NVIDIA RTX 4070 8GB, 32GB LPDDR5x, SSD 1TB, 16.3" OLED 3.2K 3200x2000 120Hz HDR 500 touch. Flagship creator laptop.',
    72990000.00, 5.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='workstation'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='dell'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'dell-xps-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Dell G16 Gaming 2025', N'dell-g16-gaming-2025',
    N'Intel Core i7-14650HX (16-nhân), NVIDIA RTX 4070 8GB, 16GB DDR5, SSD 512GB, 16" QHD+ 2560x1600 165Hz 100% sRGB. Gaming tầm trung giá tốt nhất 2025.',
    31990000.00, 8.00, 15,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='dell'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'dell-g16-gaming-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Dell Latitude 9450 2-in-1', N'dell-latitude-9450-2in1-2025',
    N'Intel Core Ultra 7 166U (12-nhân), Intel Arc iGPU, 32GB LPDDR5x, SSD 1TB, 14" 2.8K 2880x1800 60Hz OLED touch, eSIM 5G, bảo mật IR+fingerprint, 1.36kg.',
    54990000.00, 0.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='dell'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'dell-latitude-9450-2in1-2025');

-- ─── HP (4 sản phẩm bổ sung) ─────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'HP Spectre x360 14 2025', N'hp-spectre-x360-14-2025',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 32GB LPDDR5x, SSD 1TB, 14" 2.8K 2880x1800 OLED LTPO 1-120Hz HDR 500 touch, bút HP MPP 2.0, gem-cut design.',
    44990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='hp'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'hp-spectre-x360-14-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'HP Omen 16 2025', N'hp-omen-16-2025',
    N'Intel Core i9-14900HX, NVIDIA RTX 4070 8GB, 32GB DDR5, SSD 1TB, 16" QHD+ 2560x1600 240Hz 100% sRGB, OMEN Command Center, MUX Switch, tản nhiệt Tempest.',
    44990000.00, 8.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='hp'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'hp-omen-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'HP EliteBook 1040 G11', N'hp-elitebook-1040-g11-2025',
    N'Intel Core Ultra 7 165U (12-nhân), Intel Arc iGPU, 32GB LPDDR5, SSD 1TB, 14" 2.8K OLED 2880x1800 60Hz HP Sure View Reflect chống nhìn trộm, 5G, 1.24kg.',
    49990000.00, 0.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='hp'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'hp-elitebook-1040-g11-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'HP Pavilion Plus 14 2025', N'hp-pavilion-plus-14-2025',
    N'Intel Core Ultra 5 125H (14-nhân), Intel Arc iGPU, 16GB LPDDR5, SSD 512GB, 14" 2.8K OLED 2880x1800 90Hz, 1.41kg, USB-C 140W, pin 10 giờ. Tầm trung OLED.',
    22990000.00, 5.00, 20,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='hp'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'hp-pavilion-plus-14-2025');

-- ─── LENOVO ThinkPad + Legion + IdeaPad (4 sản phẩm) ──────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Lenovo ThinkPad X1 Carbon Gen 13', N'lenovo-thinkpad-x1-carbon-gen13',
    N'Qualcomm Snapdragon X Elite X1E-78-100 (12-nhân), Snapdragon X Elite GPU, 32GB LPDDR5x, SSD 1TB, 14" IPS 2.8K 2880x1800 60Hz, Copilot+, 1.09kg — nhẹ nhất T-series.',
    44990000.00, 5.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lenovo'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lenovo-thinkpad-x1-carbon-gen13');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Lenovo Legion 9i Gen 9 2025', N'lenovo-legion-9i-gen9-2025',
    N'Intel Core i9-14900HX, NVIDIA RTX 4090 16GB, 64GB DDR5 5600MHz, SSD 2TB PCIe 5.0, 16" Mini-LED QHD+ 2560x1600 165Hz 100% DCI-P3, liquid-cooled.',
    84990000.00, 0.00, 4,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lenovo'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lenovo-legion-9i-gen9-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Lenovo Yoga 9i 14 Gen 10 2025', N'lenovo-yoga-9i-14-gen10-2025',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 32GB LPDDR5x, SSD 1TB, 14" 2.8K OLED 2880x1800 120Hz 100% DCI-P3 touch, Bowers & Wilkins, 1.36kg.',
    44990000.00, 5.00, 8,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='2-in-1-convertible'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lenovo'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lenovo-yoga-9i-14-gen10-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Lenovo IdeaPad 5 16 2025', N'lenovo-ideapad-5-16-2025',
    N'AMD Ryzen 7 8845HS (8-nhân, 5.1GHz), AMD Radeon 780M iGPU, 16GB DDR5, SSD 512GB, 16" IPS 1920x1200 60Hz 300 nits, 1.69kg. Giá trị văn phòng.',
    17990000.00, 5.00, 25,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='notebook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='lenovo'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'lenovo-ideapad-5-16-2025');

-- ─── ASUS ROG + ZenBook + ProArt (4 sản phẩm) ─────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'ASUS ROG Zephyrus G16 2025', N'asus-rog-zephyrus-g16-2025',
    N'Intel Core Ultra 9 185H, NVIDIA RTX 4090 16GB, 32GB LPDDR5x, SSD 2TB, 16" OLED QHD+ 2560x1600 240Hz 100% DCI-P3 VESA HDR600, 2.1kg. Flagship slim gaming.',
    79990000.00, 5.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='asus'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'asus-rog-zephyrus-g16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'ASUS ZenBook S 16 OLED 2025', N'asus-zenbook-s16-oled-2025',
    N'AMD Ryzen AI 9 HX 370 (12-nhân, 5.1GHz), AMD Radeon 890M iGPU, 32GB LPDDR5x, SSD 1TB, 16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3, 1.5kg. Copilot+ AI.',
    34990000.00, 5.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='asus'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'asus-zenbook-s16-oled-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'ASUS ProArt P16 2025', N'asus-proart-p16-2025',
    N'AMD Ryzen AI 9 HX 370, NVIDIA RTX 4070 8GB, 32GB LPDDR5x, SSD 1TB, 16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3 touch, ASUS Dial, ProArt Creator Hub. Creator flagship.',
    55990000.00, 0.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='workstation'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='asus'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'asus-proart-p16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'ASUS ROG Flow X16 2025', N'asus-rog-flow-x16-2025',
    N'AMD Ryzen 9 8945HX, NVIDIA RTX 4080 12GB, 32GB LPDDR5x, SSD 1TB, 16" QHD+ 2560x1600 240Hz OLED touch, detachable eGPU slot, 2.1kg.',
    65990000.00, 5.00, 6,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='asus'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'asus-rog-flow-x16-2025');

-- ─── ACER (3 sản phẩm bổ sung) ───────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Acer Predator Helios Neo 16 2025', N'acer-predator-helios-neo-16-2025',
    N'Intel Core i9-14900HX (24-nhân), NVIDIA RTX 4070 8GB, 16GB DDR5, SSD 512GB, 16" WQXGA 2560x1600 165Hz 100% sRGB, MUX Switch, AeroBlade, USB4.',
    36990000.00, 8.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='gaming'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='acer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'acer-predator-helios-neo-16-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Acer Swift Go 16 OLED 2025', N'acer-swift-go-16-oled-2025',
    N'Intel Core Ultra 7 258V, Intel Arc 140V, 32GB LPDDR5x, SSD 1TB, 16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3 VESA HDR 500, 1.47kg. Ultrabook OLED to nhất.',
    28990000.00, 5.00, 12,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='acer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'acer-swift-go-16-oled-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Acer ConceptD 7 Pro 2025', N'acer-conceptd-7-pro-2025',
    N'Intel Core i7-14650HX (16-nhân), NVIDIA RTX 4070 Laptop 8GB, 32GB DDR5, SSD 1TB, 16" 4K UHD 3840x2160 60Hz 100% DCI-P3 touch, PANTONE certified. Workstation creator.',
    54990000.00, 0.00, 5,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='workstation'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='acer'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'acer-conceptd-7-pro-2025');

-- ─── GOOGLE Chromebook (2 sản phẩm) ──────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Google Pixelbook Go 2025', N'google-pixelbook-go-2025',
    N'Intel Core i5-1235U, Intel Iris Xe, 8GB LPDDR4x, eMMC 256GB, 13.3" FHD IPS 1920x1080 touch, ChromeOS, pin 12 giờ, 1.1kg. Chromebook cao cấp nhất.',
    19990000.00, 5.00, 10,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='google'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'google-pixelbook-go-2025');

INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Google Chromebook Plus', N'google-chromebook-plus-2025',
    N'MediaTek Kompanio 528, ARM Mali-G57, 8GB LPDDR4x, eMMC 128GB, 14" FHD 1920x1080 touch, ChromeOS AI features, Google Magic Eraser, pin 10 giờ, 1.7kg.',
    9990000.00, 5.00, 15,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='netbook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='google'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'google-chromebook-plus-2025');

-- ─── FUJITSU (1 sản phẩm) ────────────────────────────────────
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT N'Fujitsu Lifebook U9413', N'fujitsu-lifebook-u9413-2025',
    N'Intel Core i7-1365U (10-nhân), Intel Iris Xe, 16GB LPDDR5, SSD 512GB, 14" IPS FHD 1920x1200 anti-glare, 0.87kg — nhẹ nhất thế giới 14". Dành cho executive.',
    54990000.00, 0.00, 4,
    (SELECT TOP 1 Id FROM Categories WHERE Slug='ultrabook'),
    (SELECT TOP 1 Id FROM Brands WHERE Slug='fujitsu'), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = N'fujitsu-lifebook-u9413-2025');

-- ============================================================
-- 4. INSERT ProductSpecifications cho 50 sản phẩm mới
-- ============================================================

INSERT INTO ProductSpecifications (ProductId, CPU, RAM, Storage, GPU, Screen, OS, Ports, Weight, Battery)
SELECT p.Id, s.CPU, s.RAM, s.Storage, s.GPU, s.Screen, s.OS, s.Ports, s.Weight, s.Battery
FROM (VALUES
  (N'apple-macbook-pro-14-m4-pro',      N'Apple M4 Pro 12-core CPU, 20-core GPU, 3nm', N'24GB Unified Memory', N'SSD 512GB NVMe', N'Apple M4 Pro 20-core GPU', N'14.2" Liquid Retina XDR 3024x1964 120Hz ProMotion True Tone P3', N'macOS Sequoia', N'3x Thunderbolt 5, MagSafe 3, HDMI 2.1, SD reader, 3.5mm', N'1.55 kg', N'72Wh, 24 giờ'),
  (N'apple-macbook-pro-16-m4-max',      N'Apple M4 Max 16-core CPU, 40-core GPU, 3nm', N'48GB Unified Memory', N'SSD 1TB NVMe', N'Apple M4 Max 40-core GPU', N'16.2" Liquid Retina XDR 3456x2234 120Hz ProMotion True Tone P3', N'macOS Sequoia', N'3x Thunderbolt 5, MagSafe 3, HDMI 2.1, SD reader, 3.5mm', N'2.14 kg', N'100Wh, 24 giờ'),
  (N'apple-macbook-air-13-m4',          N'Apple M4 8-core CPU, 10-core GPU, 3nm', N'16GB Unified Memory', N'SSD 256GB NVMe', N'Apple M4 10-core GPU', N'13.6" Liquid Retina 2560x1664 60Hz True Tone P3', N'macOS Sequoia', N'2x Thunderbolt 4, MagSafe 3, 3.5mm', N'1.24 kg', N'52.6Wh, 18 giờ'),
  (N'apple-macbook-air-15-m4',          N'Apple M4 8-core CPU, 10-core GPU, 3nm', N'16GB Unified Memory', N'SSD 512GB NVMe', N'Apple M4 10-core GPU', N'15.3" Liquid Retina 2880x1864 60Hz True Tone P3', N'macOS Sequoia', N'2x Thunderbolt 4, MagSafe 3, 3.5mm', N'1.51 kg', N'66.5Wh, 18 giờ'),
  (N'microsoft-surface-laptop-7-13',   N'Snapdragon X Elite X1E-80-100 (12-nhân, 3.4GHz)', N'16GB LPDDR5x', N'SSD 512GB NVMe', N'Adreno X1-85 GPU', N'13.8" PixelSense Flow 2304x1536 120Hz HDR touch', N'Windows 11 Home (Copilot+)', N'2x USB-C (USB 3.2), 1x USB-A 3.1, Surface Connect, 3.5mm', N'1.34 kg', N'54Wh, 22 giờ'),
  (N'microsoft-surface-laptop-7-15',   N'Snapdragon X Elite X1E-80-100 (12-nhân, 3.4GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'Adreno X1-85 GPU', N'15" PixelSense Flow 2496x1664 120Hz HDR touch', N'Windows 11 Home (Copilot+)', N'2x USB-C (USB 3.2), 1x USB-A 3.1, Surface Connect, 3.5mm', N'1.65 kg', N'66Wh, 22 giờ'),
  (N'microsoft-surface-pro-11',        N'Snapdragon X Plus X1P-64-100 (10-nhân)', N'16GB LPDDR5x', N'SSD 256GB NVMe', N'Adreno X1-45 GPU', N'13" PixelSense Flow 2880x1920 120Hz OLED touch', N'Windows 11 Pro (Copilot+)', N'2x USB-C 3.2, Surface Connect, nano-SIM', N'0.89 kg (máy tính bảng)', N'53Wh, 14 giờ'),
  (N'microsoft-surface-book-4',        N'Intel Core Ultra 7 165H (22-nhân, 4.8GHz)', N'32GB LPDDR5', N'SSD 512GB NVMe', N'NVIDIA RTX 4060 8GB GDDR6', N'15" PixelSense 3240x2160 60Hz touch detachable', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.1, SD reader, 3.5mm', N'1.87 kg', N'90Wh, 15 giờ'),
  (N'microsoft-surface-laptop-studio-2',N'Intel Core i7-13700H (14-nhân, 5.0GHz)', N'32GB LPDDR5', N'SSD 512GB NVMe', N'NVIDIA RTX 4060 Ada 8GB GDDR6', N'14.4" PixelSense Flow 2400x1600 120Hz touch', N'Windows 11 Pro', N'2x Thunderbolt 4, 1x USB-A 3.1, SD reader, 3.5mm', N'1.89 kg', N'58Wh, 19 giờ'),
  (N'razer-blade-16-2025',             N'Intel Core i9-14900HX (24-nhân, 5.8GHz, 36MB)', N'32GB DDR5 5600MHz', N'SSD 2TB NVMe PCIe 5.0', N'NVIDIA RTX 5080 16GB GDDR7', N'16" QHD+ OLED 2560x1600 240Hz 100% DCI-P3 VESA HDR 400', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, SD reader, 3.5mm', N'2.14 kg', N'95.2Wh, 8 giờ'),
  (N'razer-blade-14-2025',             N'AMD Ryzen 9 8945HX (8-nhân, 5.2GHz, 32MB)', N'16GB LPDDR5x 6400MHz', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4070 Ti Super 12GB GDDR6X', N'14" QHD+ 2560x1600 240Hz 100% DCI-P3', N'Windows 11 Home', N'2x USB-C (1x TB4), 2x USB-A 3.2, HDMI 2.1, 3.5mm', N'1.65 kg', N'68Wh, 8 giờ'),
  (N'razer-blade-15-2025',             N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'32GB DDR5 5600MHz', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4080 12GB GDDR6', N'15.6" QHD 2560x1440 240Hz 100% DCI-P3', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, SD reader, 3.5mm', N'2.01 kg', N'80Wh, 8 giờ'),
  (N'razer-book-13-2025',              N'Intel Core Ultra 7 155U (22-nhân, 4.8GHz)', N'16GB LPDDR5x', N'SSD 512GB NVMe PCIe 4.0', N'Intel Arc Graphics', N'13.4" OLED 2880x1800 60Hz 100% DCI-P3 touch', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, 3.5mm', N'1.29 kg', N'55Wh, 15 giờ'),
  (N'lg-gram-17-2025',                 N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'16GB LPDDR5x', N'SSD 1TB NVMe', N'Intel Arc 140V GPU', N'17" IPS WQXGA 2560x1600 60Hz 300 nits', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD reader, 3.5mm', N'1.35 kg', N'80Wh, 25 giờ'),
  (N'lg-gram-16-2025',                 N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'Intel Arc 140V GPU', N'16" IPS 2560x1600 60Hz 350 nits', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD reader, 3.5mm', N'1.19 kg', N'77Wh, 22 giờ'),
  (N'lg-gram-pro-16-oled-2025',        N'Intel Core Ultra 7 258V', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'NVIDIA RTX 3050 6GB GDDR6', N'16" OLED WQXGA 2560x1600 120Hz HDR', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD reader, 3.5mm', N'1.34 kg', N'76Wh, 15 giờ'),
  (N'lg-gram-14-2025',                 N'Intel Core Ultra 5 226V (8-nhân, 4.5GHz)', N'16GB LPDDR5x', N'SSD 512GB NVMe', N'Intel Arc 130V GPU', N'14" IPS WUXGA 1920x1200 60Hz 400 nits', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD reader, 3.5mm', N'0.99 kg', N'72Wh, 25 giờ'),
  (N'samsung-galaxy-book5-pro-360-16', N'Intel Core Ultra 7 258V (8-nhân)', N'16GB LPDDR5x', N'SSD 512GB NVMe', N'Intel Arc 140V GPU', N'16" Dynamic AMOLED 2X 3K 2880x1800 120Hz 400 nits touch S Pen', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, HDMI, SD reader, 3.5mm', N'1.66 kg', N'76Wh, 18 giờ'),
  (N'samsung-galaxy-book5-ultra',      N'Intel Core Ultra 9 285H (24-nhân, 5.4GHz)', N'32GB LPDDR5x 7467MHz', N'SSD 1TB NVMe', N'NVIDIA RTX 4070 8GB GDDR6', N'16" Dynamic AMOLED 2X 3K 2880x1800 120Hz', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD reader, 3.5mm', N'1.86 kg', N'76Wh, 12 giờ'),
  (N'samsung-galaxy-book4-edge',       N'Snapdragon X Elite X1E-84-100 (12-nhân)', N'16GB LPDDR5x', N'SSD 512GB NVMe', N'Adreno X1-85 GPU', N'14" Dynamic AMOLED 2X 2880x1800 120Hz 500 nits', N'Windows 11 Home (Copilot+)', N'2x Thunderbolt 4, 1x USB-A 3.2, SD reader, 3.5mm', N'1.17 kg', N'63Wh, 22 giờ'),
  (N'samsung-galaxy-book4-360-15',     N'Intel Core Ultra 5 125H (14-nhân, 4.5GHz)', N'16GB LPDDR5', N'SSD 512GB NVMe', N'Intel Arc iGPU', N'15.6" FHD AMOLED 1920x1080 60Hz touch S Pen', N'Windows 11 Home', N'2x USB-C, 2x USB-A, HDMI, SD reader, 3.5mm', N'1.61 kg', N'68Wh, 14 giờ'),
  (N'msi-titan-gt77-2025',             N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'64GB DDR5 5600MHz', N'SSD 2TB PCIe 5.0 x2', N'NVIDIA RTX 5090 24GB GDDR7', N'17.3" UHD IPS 3840x2160 144Hz 100% AdobeRGB', N'Windows 11 Pro', N'2x Thunderbolt 4, 4x USB-A 3.2, 1x USB-C 3.2, HDMI 2.1, miniDP 1.4, SD, 3.5mm', N'3.1 kg', N'99.9Wh, 5 giờ'),
  (N'msi-raider-ge78-hx-2025',         N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'32GB DDR5 4800MHz', N'SSD 2TB NVMe PCIe 5.0', N'NVIDIA RTX 4090 16GB GDDR6', N'17" QHD+ 2560x1600 240Hz 100% DCI-P3', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A, HDMI 2.1, DP 2.1, RJ-45, SD, 3.5mm', N'3.1 kg', N'99.9Wh, 6 giờ'),
  (N'msi-stealth-16-studio-2025',      N'Intel Core i9-13980HX (24-nhân, 5.6GHz)', N'32GB DDR5 4800MHz', N'SSD 2TB NVMe PCIe 4.0', N'NVIDIA RTX 4080 12GB GDDR6', N'16" QHD+ OLED 2560x1600 240Hz 100% DCI-P3', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI 2.1, miniDP 1.4, SD, 3.5mm', N'1.99 kg', N'99.9Wh, 8 giờ'),
  (N'msi-prestige-16-ai-evo-2025',     N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'Intel Arc 140V GPU', N'16" IPS+ 2560x1600 120Hz sRGB 100%', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI 2.0, SD, 3.5mm', N'1.65 kg', N'72Wh, 16 giờ'),
  (N'msi-vector-17-hx-2025',           N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'32GB DDR5 5600MHz', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4080 12GB GDDR6', N'17.3" FHD IPS 1920x1080 360Hz 100% sRGB MUX Switch', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, miniDP 1.4, RJ-45, SD, 3.5mm', N'2.59 kg', N'90Wh, 7 giờ'),
  (N'huawei-matebook-x-pro-2025',      N'Intel Core Ultra 9 185H (22-nhân, 5.1GHz)', N'32GB LPDDR5', N'SSD 1TB NVMe', N'Intel Arc iGPU', N'14.2" OLED 3120x2080 LTPO 1-120Hz HDR', N'Windows 11 Pro', N'2x Thunderbolt 4, 1x USB-C 3.2, 1x USB-A 3.2, 3.5mm', N'1.26 kg', N'70Wh, 18 giờ'),
  (N'huawei-matebook-16s-2025',        N'Intel Core Ultra 9 185H (22-nhân)', N'32GB LPDDR5', N'SSD 1TB NVMe', N'Intel Arc iGPU', N'16" IPS+ 2520x1680 60Hz touch', N'Windows 11 Home', N'2x Thunderbolt 4, 2x USB-A 3.0, HDMI 2.0, 3.5mm', N'1.99 kg', N'84Wh, 12 giờ'),
  (N'huawei-matebook-d-16-2025',       N'Intel Core i7-13700H (14-nhân, 5.0GHz)', N'16GB LPDDR4x', N'SSD 512GB NVMe', N'NVIDIA MX550 2GB GDDR6', N'16" IPS FHD 1920x1200 300 nits EyeComfort', N'Windows 11 Home', N'1x USB-C 3.2, 2x USB-A 3.2, 1x USB-A 2.0, HDMI, 3.5mm', N'1.68 kg', N'60Wh, 12 giờ'),
  (N'xiaomi-mi-notebook-pro-x-14-2025',N'Intel Core Ultra 7 155H (22-nhân, 4.8GHz)', N'32GB LPDDR5', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4060 8GB GDDR6', N'14" OLED 2880x1800 120Hz 100% DCI-P3 touch', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, HDMI 2.0, 3.5mm', N'1.65 kg', N'75Wh, 12 giờ'),
  (N'xiaomi-book-pro-16-2025',         N'Intel Core i7-1360P (12-nhân, 5.0GHz)', N'16GB LPDDR5', N'SSD 512GB NVMe', N'NVIDIA MX550 2GB GDDR6', N'16" OLED 3K 3200x2000 120Hz VESA HDR 600', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, HDMI 2.0, 3.5mm', N'1.8 kg', N'75Wh, 12 giờ'),
  (N'xiaomi-redmi-book-16-2025',       N'AMD Ryzen 5 8645HS (6-nhân, 5.0GHz)', N'16GB LPDDR5', N'SSD 512GB NVMe', N'AMD Radeon 760M iGPU', N'16" FHD IPS 1920x1200 144Hz 100% sRGB', N'Windows 11 Home', N'1x USB-C, 2x USB-A 3.2, HDMI, 3.5mm', N'1.58 kg', N'72Wh, 10 giờ'),
  (N'dell-xps-14-2025',                N'Intel Core Ultra 7 155H (22-nhân, 4.8GHz)', N'16GB LPDDR5x 7467MHz', N'SSD 512GB NVMe PCIe 4.0', N'NVIDIA RTX 4050 6GB GDDR6', N'14.5" OLED 3.2K 3200x2000 120Hz HDR 500 touch', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-C 3.2, SD reader, 3.5mm', N'1.64 kg', N'69.5Wh, 12 giờ'),
  (N'dell-xps-16-2025',                N'Intel Core Ultra 9 185H (22-nhân, 5.1GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4070 8GB GDDR6', N'16.3" OLED 3.2K 3200x2000 120Hz HDR 500 touch', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-C 3.2, SD reader, 3.5mm', N'1.89 kg', N'99.5Wh, 14 giờ'),
  (N'dell-g16-gaming-2025',            N'Intel Core i7-14650HX (16-nhân, 5.2GHz)', N'16GB DDR5 4800MHz', N'SSD 512GB NVMe PCIe 4.0', N'NVIDIA RTX 4070 8GB GDDR6', N'16" QHD+ 2560x1600 165Hz 100% sRGB', N'Windows 11 Home', N'1x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, RJ-45, SD, 3.5mm', N'2.91 kg', N'86Wh, 8 giờ'),
  (N'dell-latitude-9450-2in1-2025',    N'Intel Core Ultra 7 166U (12-nhân, 4.4GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe PCIe 4.0', N'Intel Arc iGPU', N'14" OLED 2.8K 2880x1800 60Hz touch', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-C 3.2, nano-SIM (5G)', N'1.36 kg', N'57Wh, 14 giờ'),
  (N'hp-spectre-x360-14-2025',         N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe PCIe 4.0', N'Intel Arc 140V GPU', N'14" OLED LTPO 2880x1800 1-120Hz HDR 500 touch + bút MPP 2.0', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, 3.5mm', N'1.41 kg', N'66Wh, 17 giờ'),
  (N'hp-omen-16-2025',                 N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'32GB DDR5 4800MHz', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4070 8GB GDDR6', N'16" QHD+ 2560x1600 240Hz 100% sRGB MUX Switch', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, RJ-45, SD, 3.5mm', N'2.35 kg', N'83Wh, 8 giờ'),
  (N'hp-elitebook-1040-g11-2025',      N'Intel Core Ultra 7 165U (12-nhân, 4.8GHz)', N'32GB LPDDR5', N'SSD 1TB NVMe PCIe 4.0', N'Intel Arc iGPU', N'14" OLED 2.8K 2880x1800 60Hz HP Sure View Reflect', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, nano-SIM', N'1.24 kg', N'51Wh, 15 giờ'),
  (N'hp-pavilion-plus-14-2025',        N'Intel Core Ultra 5 125H (14-nhân, 4.5GHz)', N'16GB LPDDR5', N'SSD 512GB NVMe', N'Intel Arc iGPU', N'14" OLED 2.8K 2880x1800 90Hz 100% DCI-P3', N'Windows 11 Home', N'1x USB-C 3.2, 2x USB-A 3.2, HDMI, SD, 3.5mm', N'1.41 kg', N'43Wh, 10 giờ'),
  (N'lenovo-thinkpad-x1-carbon-gen13', N'Snapdragon X Elite X1E-78-100 (12-nhân, 3.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe PCIe 4.0', N'Adreno X1-85 GPU', N'14" IPS 2.8K 2880x1800 60Hz anti-reflective', N'Windows 11 Pro (Copilot+)', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI 2.1, 3.5mm', N'1.09 kg', N'57Wh, 15 giờ'),
  (N'lenovo-legion-9i-gen9-2025',      N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'64GB DDR5 5600MHz', N'SSD 2TB NVMe PCIe 5.0', N'NVIDIA RTX 4090 16GB GDDR6', N'16" Mini-LED QHD+ 2560x1600 165Hz 100% DCI-P3', N'Windows 11 Home', N'2x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, RJ-45, SD, 3.5mm', N'2.5 kg', N'99.9Wh, 6 giờ'),
  (N'lenovo-yoga-9i-14-gen10-2025',    N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'Intel Arc 140V GPU', N'14" OLED 2.8K 2880x1800 120Hz 100% DCI-P3 touch', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, 3.5mm', N'1.36 kg', N'70Wh, 18 giờ'),
  (N'lenovo-ideapad-5-16-2025',        N'AMD Ryzen 7 8845HS (8-nhân, 5.1GHz)', N'16GB DDR5', N'SSD 512GB NVMe', N'AMD Radeon 780M iGPU', N'16" IPS 1920x1200 60Hz 300 nits', N'Windows 11 Home', N'1x USB-C 3.2, 2x USB-A 3.2, HDMI, 3.5mm', N'1.69 kg', N'60Wh, 10 giờ'),
  (N'asus-rog-zephyrus-g16-2025',      N'Intel Core Ultra 9 185H (22-nhân, 5.1GHz)', N'32GB LPDDR5x 7467MHz', N'SSD 2TB NVMe PCIe 4.0', N'NVIDIA RTX 4090 16GB GDDR6', N'16" OLED QHD+ 2560x1600 240Hz 100% DCI-P3 VESA HDR 600', N'Windows 11 Home', N'1x Thunderbolt 4, 1x USB-C 3.2, 2x USB-A 3.2, HDMI 2.1, SD, 3.5mm', N'2.1 kg', N'90Wh, 8 giờ'),
  (N'asus-zenbook-s16-oled-2025',      N'AMD Ryzen AI 9 HX 370 (12-nhân, 5.1GHz)', N'32GB LPDDR5x 7500MHz', N'SSD 1TB NVMe PCIe 4.0', N'AMD Radeon 890M iGPU', N'16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3 VESA HDR 500', N'Windows 11 Home (Copilot+)', N'2x USB-C 3.2, 2x USB-A 3.2, HDMI, SD, 3.5mm', N'1.5 kg', N'78Wh, 15 giờ'),
  (N'asus-proart-p16-2025',            N'AMD Ryzen AI 9 HX 370 (12-nhân, 5.1GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4070 8GB GDDR6', N'16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3 touch ASUS Dial', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI, SD, 3.5mm', N'1.8 kg', N'90Wh, 12 giờ'),
  (N'asus-rog-flow-x16-2025',          N'AMD Ryzen 9 8945HX (8-nhân, 5.2GHz)', N'32GB LPDDR5x 7500MHz', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4080 12GB GDDR6', N'16" QHD+ OLED 2560x1600 240Hz touch 100% DCI-P3', N'Windows 11 Home', N'1x USB-C 3.2, 2x USB-A 3.2, HDMI 2.1, SD, 3.5mm, XG port', N'2.1 kg', N'90Wh, 8 giờ'),
  (N'acer-predator-helios-neo-16-2025',N'Intel Core i9-14900HX (24-nhân, 5.8GHz)', N'16GB DDR5 4800MHz', N'SSD 512GB NVMe PCIe 4.0', N'NVIDIA RTX 4070 8GB GDDR6', N'16" WQXGA 2560x1600 165Hz 100% sRGB IPS', N'Windows 11 Home', N'1x Thunderbolt 4, 3x USB-A 3.2, HDMI 2.1, USB4, RJ-45, SD, 3.5mm', N'2.7 kg', N'90Wh, 7 giờ'),
  (N'acer-swift-go-16-oled-2025',      N'Intel Core Ultra 7 258V (8-nhân, 4.8GHz)', N'32GB LPDDR5x', N'SSD 1TB NVMe', N'Intel Arc 140V GPU', N'16" OLED 3.2K 3200x2000 120Hz 100% DCI-P3 VESA HDR 500', N'Windows 11 Home', N'2x Thunderbolt 4, 1x USB-A 3.2, HDMI, SD, 3.5mm', N'1.47 kg', N'75Wh, 14 giờ'),
  (N'acer-conceptd-7-pro-2025',        N'Intel Core i7-14650HX (16-nhân, 5.2GHz)', N'32GB DDR5', N'SSD 1TB NVMe PCIe 4.0', N'NVIDIA RTX 4070 Laptop 8GB GDDR6', N'16" 4K UHD 3840x2160 60Hz 100% DCI-P3 touch PANTONE', N'Windows 11 Pro', N'2x Thunderbolt 4, 2x USB-A 3.2, HDMI 2.0, SD, 3.5mm', N'2.1 kg', N'88Wh, 10 giờ'),
  (N'google-pixelbook-go-2025',        N'Intel Core i5-1235U (10-nhân, 4.4GHz)', N'8GB LPDDR4x', N'eMMC 256GB', N'Intel Iris Xe Graphics', N'13.3" FHD IPS 1920x1080 touch', N'ChromeOS', N'2x USB-C 3.2, 3.5mm', N'1.1 kg', N'47Wh, 12 giờ'),
  (N'google-chromebook-plus-2025',     N'MediaTek Kompanio 528 (8-nhân)', N'8GB LPDDR4x', N'eMMC 128GB', N'ARM Mali-G57 MC5 GPU', N'14" FHD IPS 1920x1080 touch ChromeOS AI', N'ChromeOS', N'2x USB-C 3.2, 1x USB-A 3.0, 3.5mm, microSD', N'1.7 kg', N'48Wh, 10 giờ'),
  (N'fujitsu-lifebook-u9413-2025',     N'Intel Core i7-1365U (10-nhân, 5.2GHz)', N'16GB LPDDR5', N'SSD 512GB NVMe', N'Intel Iris Xe Graphics', N'14" IPS FHD 1920x1200 anti-glare matte 400 nits', N'Windows 11 Pro', N'2x Thunderbolt 4, 1x USB-A 3.2, HDMI, SD, smartcard, 3.5mm', N'0.87 kg', N'50Wh, 12 giờ')
) AS s(Slug, CPU, RAM, Storage, GPU, Screen, OS, Ports, Weight, Battery)
JOIN Products p ON p.Slug = s.Slug
WHERE NOT EXISTS (SELECT 1 FROM ProductSpecifications ps WHERE ps.ProductId = p.Id);

-- ============================================================
-- 5. INSERT Inventories cho 50 sản phẩm mới
-- ============================================================

INSERT INTO Inventories (ProductId, CurrentStock, MinimumStock, ReorderPoint, AverageCost, LastPurchasePrice, LastUpdated, Location, Status)
SELECT p.Id, inv.Stock, 3, 8, inv.Cost, inv.Cost, GETUTCDATE(), N'Kho Hà Nội', 1
FROM (VALUES
  (N'apple-macbook-pro-14-m4-pro',      10, 53000000),
  (N'apple-macbook-pro-16-m4-max',       6, 90000000),
  (N'apple-macbook-air-13-m4',          30, 26000000),
  (N'apple-macbook-air-15-m4',          20, 33000000),
  (N'microsoft-surface-laptop-7-13',    12, 31000000),
  (N'microsoft-surface-laptop-7-15',     8, 40000000),
  (N'microsoft-surface-pro-11',         10, 34000000),
  (N'microsoft-surface-book-4',          5, 64000000),
  (N'microsoft-surface-laptop-studio-2', 6, 68000000),
  (N'razer-blade-16-2025',               4, 92000000),
  (N'razer-blade-14-2025',               5, 63000000),
  (N'razer-blade-15-2025',               4, 77000000),
  (N'razer-book-13-2025',                8, 36000000),
  (N'lg-gram-17-2025',                  10, 36000000),
  (N'lg-gram-16-2025',                  12, 32000000),
  (N'lg-gram-pro-16-oled-2025',          8, 42000000),
  (N'lg-gram-14-2025',                  15, 27000000),
  (N'samsung-galaxy-book5-pro-360-16',  10, 40000000),
  (N'samsung-galaxy-book5-ultra',        6, 54000000),
  (N'samsung-galaxy-book4-edge',        12, 31000000),
  (N'samsung-galaxy-book4-360-15',      15, 25000000),
  (N'msi-titan-gt77-2025',               3, 118000000),
  (N'msi-raider-ge78-hx-2025',           4, 73000000),
  (N'msi-stealth-16-studio-2025',        5, 59000000),
  (N'msi-prestige-16-ai-evo-2025',      10, 30000000),
  (N'msi-vector-17-hx-2025',             5, 53000000),
  (N'huawei-matebook-x-pro-2025',        8, 41000000),
  (N'huawei-matebook-16s-2025',         10, 33000000),
  (N'huawei-matebook-d-16-2025',        15, 20000000),
  (N'xiaomi-mi-notebook-pro-x-14-2025', 12, 32000000),
  (N'xiaomi-book-pro-16-2025',          10, 26000000),
  (N'xiaomi-redmi-book-16-2025',        20, 14000000),
  (N'dell-xps-14-2025',                  8, 48000000),
  (N'dell-xps-16-2025',                  5, 67000000),
  (N'dell-g16-gaming-2025',             15, 29000000),
  (N'dell-latitude-9450-2in1-2025',      6, 50000000),
  (N'hp-spectre-x360-14-2025',          10, 40000000),
  (N'hp-omen-16-2025',                  10, 41000000),
  (N'hp-elitebook-1040-g11-2025',        6, 45000000),
  (N'hp-pavilion-plus-14-2025',         20, 21000000),
  (N'lenovo-thinkpad-x1-carbon-gen13',   8, 40000000),
  (N'lenovo-legion-9i-gen9-2025',        4, 77000000),
  (N'lenovo-yoga-9i-14-gen10-2025',      8, 40000000),
  (N'lenovo-ideapad-5-16-2025',         25, 16000000),
  (N'asus-rog-zephyrus-g16-2025',        5, 73000000),
  (N'asus-zenbook-s16-oled-2025',       12, 31000000),
  (N'asus-proart-p16-2025',              6, 51000000),
  (N'asus-rog-flow-x16-2025',            6, 60000000),
  (N'acer-predator-helios-neo-16-2025', 12, 33000000),
  (N'acer-swift-go-16-oled-2025',       12, 26000000),
  (N'acer-conceptd-7-pro-2025',          5, 50000000),
  (N'google-pixelbook-go-2025',         10, 18000000),
  (N'google-chromebook-plus-2025',      15,  9000000),
  (N'fujitsu-lifebook-u9413-2025',       4, 50000000)
) AS inv(Slug, Stock, Cost)
JOIN Products p ON p.Slug = inv.Slug
WHERE NOT EXISTS (SELECT 1 FROM Inventories i WHERE i.ProductId = p.Id);

COMMIT TRANSACTION;

PRINT '=== 10_seed_products_2026 hoàn tất ===';
SELECT 'Products' AS [Table], COUNT(*) AS [Count] FROM Products
UNION ALL SELECT 'ProductSpecifications', COUNT(*) FROM ProductSpecifications
UNION ALL SELECT 'Inventories', COUNT(*) FROM Inventories;

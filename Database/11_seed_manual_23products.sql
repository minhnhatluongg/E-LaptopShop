-- ==============================================
-- E-LaptopShop - Seed Data Script
-- Tables: Brands, Categories, Products,
--         ProductSpecifications, Inventories
-- Run on: Azure SQL (be-laptopshop.database.windows.net)
-- 23 sản phẩm laptop thực tế
-- ==============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

-- ============================================================
-- 1. BRANDS (10 thương hiệu)
-- ============================================================
INSERT INTO Brands (Name, Description, Slug, IsActive, CreatedAt)
SELECT src.Name, src.Description, src.Slug, src.IsActive, GETUTCDATE()
FROM (VALUES
    (N'Apple',   N'Thương hiệu công nghệ hàng đầu thế giới, nổi tiếng với dòng MacBook hiệu suất cao và hệ sinh thái Apple độc đáo', 'apple', 1),
    (N'Dell',    N'Dell Technologies – laptop cao cấp từ dòng XPS mỏng nhẹ đến gaming Alienware hiệu suất cực mạnh', 'dell', 1),
    (N'HP',      N'HP Inc. – Hewlett-Packard, thương hiệu laptop đa dạng từ văn phòng EliteBook đến gaming Victus', 'hp', 1),
    (N'Lenovo',  N'Lenovo Group – laptop ThinkPad bền bỉ cho doanh nghiệp và Legion gaming hiệu suất cao', 'lenovo', 1),
    (N'ASUS',    N'ASUS – laptop ROG Strix cho game thủ chuyên nghiệp và ZenBook mỏng nhẹ cao cấp', 'asus', 1),
    (N'Acer',    N'Acer Inc. – laptop phổ thông giá tốt với dòng gaming Nitro và văn phòng Swift siêu nhẹ', 'acer', 1),
    (N'MSI',     N'MSI – Micro-Star International, chuyên laptop gaming cao cấp với tản nhiệt Cooler Boost ưu việt', 'msi', 1),
    (N'Samsung', N'Samsung Electronics – laptop Galaxy Book cao cấp với màn hình Dynamic AMOLED sắc nét', 'samsung', 1),
    (N'LG',      N'LG Electronics – laptop LG Gram siêu nhẹ dưới 1kg, pin trâu, đạt chuẩn quân đội MIL-STD-810H', 'lg', 1),
    (N'Razer',   N'Razer – laptop gaming cao cấp nhất thị trường, chassis CNC nhôm nguyên khối dành cho game thủ chuyên nghiệp', 'razer', 1)
) AS src(Name, Description, Slug, IsActive)
WHERE NOT EXISTS (SELECT 1 FROM Brands b WHERE b.Slug = src.Slug);

-- ============================================================
-- 2. CATEGORIES (1 cha + 5 con)
-- ============================================================
-- Parent
INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'Laptop', 'laptop', N'Tất cả các dòng laptop', NULL, 1, 0, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0);

-- Children
INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'Laptop Gaming', 'laptop-gaming', N'Laptop gaming hiệu suất cao dành cho game thủ chuyên nghiệp',
    (SELECT Id FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0), 1, 1, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptop-gaming' AND IsDeleted = 0);

INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'Laptop Văn Phòng', 'laptop-van-phong', N'Laptop văn phòng mỏng nhẹ, pin lâu cho công việc hàng ngày',
    (SELECT Id FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0), 1, 2, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptop-van-phong' AND IsDeleted = 0);

INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'Laptop Đồ Họa', 'laptop-do-hoa', N'Laptop đồ họa màn hình calibrated, GPU mạnh cho thiết kế và dựng phim chuyên nghiệp',
    (SELECT Id FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0), 1, 3, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptop-do-hoa' AND IsDeleted = 0);

INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'Laptop Mỏng Nhẹ', 'laptop-mong-nhe', N'Laptop ultrabook mỏng nhẹ, thiết kế sang trọng, pin dài cho người di chuyển nhiều',
    (SELECT Id FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0), 1, 4, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptop-mong-nhe' AND IsDeleted = 0);

INSERT INTO Categories (Name, Slug, Description, ParentId, IsActive, DisplayOrder, CreatedAt, IsDeleted)
SELECT N'MacBook', 'macbook', N'Dòng MacBook chính hãng Apple với chip M-series hiệu suất vượt trội, tiết kiệm điện',
    (SELECT Id FROM Categories WHERE Slug = 'laptop' AND IsDeleted = 0), 1, 5, GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'macbook' AND IsDeleted = 0);

-- ============================================================
-- 3. PRODUCTS (23 sản phẩm)
-- ============================================================
INSERT INTO Products (Name, Slug, Description, Price, Discount, InStock, CategoryId, BrandId, IsActive, CreatedAt)
SELECT p.Name, p.Slug, p.Description, p.Price, p.Discount, p.InStock,
    (SELECT Id FROM Categories WHERE Slug = p.CatSlug AND IsDeleted = 0),
    (SELECT Id FROM Brands       WHERE Slug = p.BrandSlug),
    1, GETUTCDATE()
FROM (VALUES
    -- Apple MacBook
    (N'Apple MacBook Pro 14" M3 Pro', 'apple-macbook-pro-14-m3-pro',
     N'MacBook Pro 14" chip M3 Pro 11 nhân CPU – 14 nhân GPU, 18GB Unified Memory, màn Liquid Retina XDR 3024×1964 120Hz ProMotion, pin lên đến 18 giờ. Hiệu suất đỉnh cao cho lập trình viên và nhà sáng tạo nội dung chuyên nghiệp.',
     52990000, 5.00, 12, 'macbook', 'apple'),

    (N'Apple MacBook Pro 16" M3 Max', 'apple-macbook-pro-16-m3-max',
     N'MacBook Pro 16" chip M3 Max 14 nhân CPU – 30 nhân GPU, 36GB Unified Memory, màn Liquid Retina XDR 3456×2234 120Hz ProMotion, pin lên đến 22 giờ. Render video 4K/8K và machine learning đẳng cấp chưa từng có.',
     89990000, 3.00, 8, 'macbook', 'apple'),

    (N'Apple MacBook Air 13" M2', 'apple-macbook-air-13-m2',
     N'MacBook Air 13" chip M2 8 nhân CPU – 8 nhân GPU, 8GB Unified Memory, SSD 256GB, màn Liquid Retina 2560×1664 500 nits. Thiết kế fanless hoàn toàn im lặng, trọng lượng chỉ 1.24kg, lý tưởng cho sinh viên và người dùng phổ thông.',
     27990000, 10.00, 25, 'macbook', 'apple'),

    (N'Apple MacBook Air 15" M3', 'apple-macbook-air-15-m3',
     N'MacBook Air 15" chip M3 8 nhân CPU – 10 nhân GPU, 8GB Unified Memory, màn 15.3" Liquid Retina 2880×1864 500 nits. Màn hình lớn nhất dòng Air, pin 18 giờ, trọng lượng 1.51kg, không quạt tản nhiệt.',
     34990000, 5.00, 18, 'macbook', 'apple'),

    -- Dell
    (N'Dell XPS 15 9530', 'dell-xps-15-9530',
     N'Dell XPS 15 Core i7-13700H 14 nhân, RTX 4060 8GB, 16GB DDR5, màn OLED 3.5K 3456×2160 60Hz 100% DCI-P3. Chassis nhôm carbon fiber cao cấp, màn sắc nét tuyệt vời cho thiết kế đồ họa và video editing chuyên nghiệp.',
     45000000, 8.00, 10, 'laptop-do-hoa', 'dell'),

    (N'Dell XPS 13 Plus', 'dell-xps-13-plus',
     N'Dell XPS 13 Plus Core i7-1360P 12 nhân, 16GB LPDDR5, màn OLED 3.5K 3456×2160 60Hz. Siêu mỏng 15.28mm, nặng 1.26kg với touchpad haptic không viền và hàng phím cảm ứng function độc đáo.',
     33000000, 5.00, 15, 'laptop-mong-nhe', 'dell'),

    (N'Dell Alienware m16 R2', 'dell-alienware-m16-r2',
     N'Dell Alienware m16 R2 Core i9-14900HX 24 nhân, RTX 4080 12GB, 32GB DDR5 5600MHz, màn QHD+ 2560×1600 240Hz 100% DCI-P3. Con quái vật gaming với Cherry MeCool tản nhiệt tiên tiến và Cherry MX Ultra Low Profile Keyboard.',
     58000000, 0.00, 6, 'laptop-gaming', 'dell'),

    (N'Dell Inspiron 15 3530', 'dell-inspiron-15-3530',
     N'Dell Inspiron 15 Core i5-1335U 10 nhân, Intel Iris Xe, 8GB DDR4, SSD 256GB, màn FHD 15.6" 60Hz. Laptop văn phòng đáng tin cậy cho sinh viên và nhân viên công sở với giá thành hợp lý nhất phân khúc.',
     17500000, 0.00, 30, 'laptop-van-phong', 'dell'),

    -- HP
    (N'HP Spectre x360 14', 'hp-spectre-x360-14',
     N'HP Spectre x360 14 Core Ultra 7 155H 22 nhân, 16GB LPDDR5x, màn OLED 2.8K 2880×1800 120Hz 100% DCI-P3. Laptop 2-in-1 cao cấp nhất của HP với bút HP MPP 2.0 kèm theo, thiết kế gem-cut sang trọng.',
     38000000, 5.00, 14, 'laptop-mong-nhe', 'hp'),

    (N'HP Victus 16', 'hp-victus-16',
     N'HP Victus 16 Ryzen 7 7700HX 8 nhân, RTX 4060 8GB, 16GB DDR5, SSD 512GB, màn FHD 144Hz. Gaming laptop tầm trung giá trị tốt nhất với tản nhiệt Thermal Grizzly Kryonaut và RGB backlit keyboard.',
     24500000, 5.00, 20, 'laptop-gaming', 'hp'),

    (N'HP EliteBook 840 G10', 'hp-elitebook-840-g10',
     N'HP EliteBook 840 G10 Core i7-1355U 10 nhân, 16GB DDR5, SSD 512GB, màn Sure View 14" FHD 400 nits. Laptop doanh nghiệp bảo mật cao với tính năng chống nhìn trộm Sure View tích hợp, đạt chuẩn MIL-STD-810H.',
     31000000, 0.00, 12, 'laptop-van-phong', 'hp'),

    -- Lenovo
    (N'Lenovo ThinkPad X1 Carbon Gen 11', 'lenovo-thinkpad-x1-carbon-gen11',
     N'ThinkPad X1 Carbon Gen 11 Core i7-1365U 10 nhân, 16GB LPDDR5, màn IPS 2.8K 2880×1800 60Hz 100% sRGB. Laptop doanh nhân nhẹ nhất chỉ 1.12kg, đạt 12 chuẩn MIL-SPEC, bàn phím ThinkPad huyền thoại.',
     40000000, 5.00, 10, 'laptop-van-phong', 'lenovo'),

    (N'Lenovo Legion 5 Pro', 'lenovo-legion-5-pro',
     N'Lenovo Legion 5 Pro Ryzen 7 7745HX 8 nhân, RTX 4070 8GB, 16GB DDR5, màn QHD+ 2560×1600 165Hz 100% sRGB. Giá trị tốt nhất phân khúc gaming tầm cao với Legion ColdFront 5.0 tản nhiệt vượt trội.',
     36500000, 5.00, 15, 'laptop-gaming', 'lenovo'),

    (N'Lenovo IdeaPad Slim 5', 'lenovo-ideapad-slim-5',
     N'Lenovo IdeaPad Slim 5 Ryzen 5 7530U 6 nhân, 16GB DDR4, SSD 512GB, màn FHD 14" IPS 300 nits. Laptop sinh viên văn phòng giá hợp lý, mỏng nhẹ 1.46kg, pin 56Wh lên đến 8 giờ làm việc.',
     15000000, 0.00, 35, 'laptop-van-phong', 'lenovo'),

    -- ASUS
    (N'ASUS ROG Strix G16 2024', 'asus-rog-strix-g16-2024',
     N'ASUS ROG Strix G16 Core i9-14900HX 24 nhân, RTX 4070 8GB, 32GB DDR5, màn QHD+ 2560×1600 240Hz 100% DCI-P3. Gaming laptop mạnh mẽ với ROG Intelligent Cooling, Tri-Fan Technology và Keystone II độc quyền.',
     42000000, 5.00, 12, 'laptop-gaming', 'asus'),

    (N'ASUS ZenBook 14 OLED', 'asus-zenbook-14-oled',
     N'ASUS ZenBook 14 OLED Core Ultra 7 155H, 16GB LPDDR5x, màn OLED 2.8K 2880×1800 90Hz 100% DCI-P3, đạt VESA DisplayHDR True Black 500. Ultrabook 1.2kg với Eye Care display chăm sóc mắt hàng đầu.',
     22000000, 8.00, 20, 'laptop-mong-nhe', 'asus'),

    (N'ASUS ProArt Studiobook 16 OLED', 'asus-proart-studiobook-16-oled',
     N'ASUS ProArt Studiobook 16 Core i9-13980HX 24 nhân, RTX 4070 8GB, 32GB DDR5, SSD 2TB, màn OLED 3.2K 3200×2000 60Hz 100% DCI-P3 cảm ứng. Laptop đồ họa chuyên nghiệp với ASUS Dial và ProArt Creator Hub.',
     54000000, 0.00, 7, 'laptop-do-hoa', 'asus'),

    -- Acer
    (N'Acer Nitro 5 AN515 2024', 'acer-nitro-5-an515-2024',
     N'Acer Nitro 5 Core i5-13420H 12 nhân, RTX 4060 8GB, 8GB DDR5, SSD 512GB, màn FHD 165Hz. Gaming laptop entry-level tốt nhất phân khúc với NitroCool tản nhiệt kép và RGB backlit keyboard 4 vùng.',
     22000000, 5.00, 25, 'laptop-gaming', 'acer'),

    (N'Acer Swift 3 SF314', 'acer-swift-3-sf314',
     N'Acer Swift 3 Ryzen 5 7530U 6 nhân, 8GB LPDDR4X, SSD 512GB, màn FHD 14" IPS 300 nits. Laptop văn phòng mỏng nhẹ 1.25kg, vỏ nhôm bền chắc, pin lên đến 10 giờ với mức giá phải chăng.',
     12500000, 0.00, 40, 'laptop-van-phong', 'acer'),

    -- MSI
    (N'MSI Katana 15 B13VGK', 'msi-katana-15-b13vgk',
     N'MSI Katana 15 Core i7-13620H 16 nhân, RTX 4070 8GB, 16GB DDR5, SSD 512GB, màn FHD 144Hz. Gaming laptop tầm trung hiệu suất mạnh với Cooler Boost 5 tản nhiệt và MSI Center tối ưu hiệu năng.',
     30000000, 5.00, 18, 'laptop-gaming', 'msi'),

    -- Razer
    (N'Razer Blade 15 2024', 'razer-blade-15-2024',
     N'Razer Blade 15 Core i9-13950HX 24 nhân, RTX 4070 Ti Super 16GB GDDR6X, 16GB DDR5 5600MHz, SSD 1TB, màn QHD 2560×1440 240Hz 100% DCI-P3. Laptop gaming đỉnh cao, chassis CNC nhôm 0.65mm siêu bền, Thunderbolt 4 kép.',
     75000000, 0.00, 5, 'laptop-gaming', 'razer'),

    -- LG
    (N'LG Gram 14 2024', 'lg-gram-14-2024',
     N'LG Gram 14 Core Ultra 7 155H, 16GB LPDDR5x, SSD 512GB, màn IPS WUXGA 1920×1200 400 nits. Laptop siêu nhẹ chỉ 999g, đạt 7 chuẩn MIL-STD-810H, pin 72Wh lên đến 25 giờ thực tế ngoạn mục.',
     28000000, 5.00, 15, 'laptop-mong-nhe', 'lg'),

    -- Samsung
    (N'Samsung Galaxy Book4 Pro 14"', 'samsung-galaxy-book4-pro-14',
     N'Samsung Galaxy Book4 Pro 14" Core Ultra 7 155H, 16GB LPDDR5x, SSD 512GB, màn Dynamic AMOLED 2X 3K 2880×1800 120Hz 100% DCI-P3. Laptop 1.17kg với Galaxy AI tích hợp, kết nối mượt mà cùng smartphone Samsung.',
     33000000, 5.00, 15, 'laptop-mong-nhe', 'samsung')
) AS p(Name, Slug, Description, Price, Discount, InStock, CatSlug, BrandSlug)
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = p.Slug);

-- ============================================================
-- 4. PRODUCT SPECIFICATIONS
-- ============================================================
INSERT INTO ProductSpecifications (ProductId, CPU, RAM, Storage, GPU, Screen, OS, Ports, Weight, Battery)
SELECT pr.Id, s.CPU, s.RAM, s.Storage, s.GPU, s.Screen, s.OS, s.Ports, s.Weight, s.Battery
FROM (VALUES
    ('apple-macbook-pro-14-m3-pro',
     'Apple M3 Pro 11-core CPU, 14-core GPU, 3nm', '18GB Unified Memory',
     'SSD 512GB NVMe', 'Apple M3 Pro 14-core GPU (integrated)',
     N'14.2" Liquid Retina XDR 3024×1964, 120Hz ProMotion, 1600 nits peak, True Tone, P3 wide',
     'macOS Sonoma', '3× Thunderbolt 4, HDMI 2.1, SD card reader, MagSafe 3, 3.5mm', '1.61 kg', N'70Wh, lên đến 18 giờ'),

    ('apple-macbook-pro-16-m3-max',
     'Apple M3 Max 14-core CPU, 30-core GPU, 3nm', '36GB Unified Memory',
     'SSD 1TB NVMe', 'Apple M3 Max 30-core GPU (integrated)',
     N'16.2" Liquid Retina XDR 3456×2234, 120Hz ProMotion, 1600 nits peak, True Tone, P3 wide',
     'macOS Sonoma', '3× Thunderbolt 4, HDMI 2.1, SD card reader, MagSafe 3, 3.5mm', '2.15 kg', N'100Wh, lên đến 22 giờ'),

    ('apple-macbook-air-13-m2',
     'Apple M2 8-core CPU, 8-core GPU, 5nm', '8GB Unified Memory',
     'SSD 256GB NVMe', 'Apple M2 8-core GPU (integrated)',
     N'13.6" Liquid Retina 2560×1664, 60Hz, 500 nits, True Tone, P3 wide',
     'macOS Sonoma', '2× Thunderbolt 4, MagSafe 3, 3.5mm', '1.24 kg', N'52.6Wh, lên đến 18 giờ'),

    ('apple-macbook-air-15-m3',
     'Apple M3 8-core CPU, 10-core GPU, 3nm', '8GB Unified Memory',
     'SSD 256GB NVMe', 'Apple M3 10-core GPU (integrated)',
     N'15.3" Liquid Retina 2880×1864, 60Hz, 500 nits, True Tone, P3 wide',
     'macOS Sonoma', '2× Thunderbolt 4, MagSafe 3, 3.5mm', '1.51 kg', N'66.5Wh, lên đến 18 giờ'),

    ('dell-xps-15-9530',
     'Intel Core i7-13700H (14-core, up to 5.0GHz, 24MB Cache)', '16GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4060 8GB GDDR6',
     N'15.6" OLED 3.5K 3456×2160, 60Hz, 400 nits, 100% DCI-P3, cảm ứng tùy chọn',
     'Windows 11 Home', '2× Thunderbolt 4, 1× USB-A 3.2, SD card reader, 3.5mm', '1.86 kg', N'86Wh, lên đến 13 giờ'),

    ('dell-xps-13-plus',
     'Intel Core i7-1360P (12-core, up to 5.0GHz, 18MB Cache)', '16GB LPDDR5 5200MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Iris Xe Graphics',
     N'13.4" OLED 3.5K 3456×2160, 60Hz, 400 nits, 100% DCI-P3, cảm ứng',
     'Windows 11 Home', '2× Thunderbolt 4', '1.26 kg', N'55Wh, lên đến 12 giờ'),

    ('dell-alienware-m16-r2',
     'Intel Core i9-14900HX (24-core, up to 5.8GHz, 36MB Cache)', '32GB DDR5 5600MHz (2×16GB)',
     'SSD 1TB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4080 12GB GDDR6',
     N'16" QHD+ 2560×1600, 240Hz, 500 nits, 100% DCI-P3, IPS',
     'Windows 11 Home', '1× Thunderbolt 4, 3× USB-A 3.2, HDMI 2.1, Mini-DP 1.4, RJ-45, 3.5mm', '3.40 kg', N'90Wh, lên đến 6 giờ'),

    ('dell-inspiron-15-3530',
     'Intel Core i5-1335U (10-core, up to 4.6GHz, 12MB Cache)', '8GB DDR4 3200MHz',
     'SSD 256GB NVMe', 'Intel Iris Xe Graphics',
     N'15.6" FHD 1920×1080, 60Hz, 250 nits, WVA',
     'Windows 11 Home', '1× USB-C 3.2, 2× USB-A 3.2, HDMI 1.4, SD card reader, RJ-45, 3.5mm', '1.75 kg', N'54Wh, lên đến 8 giờ'),

    ('hp-spectre-x360-14',
     'Intel Core Ultra 7 155H (22-core, up to 4.8GHz, 24MB Cache)', '16GB LPDDR5x 7467MHz',
     'SSD 1TB NVMe PCIe 4.0', 'Intel Arc Graphics',
     N'14" OLED 2.8K 2880×1800, 120Hz, 400 nits, 100% DCI-P3, cảm ứng, bút MPP 2.0',
     'Windows 11 Home', '2× Thunderbolt 4, 1× USB-A 3.2, 3.5mm', '1.47 kg', N'66Wh, lên đến 17 giờ'),

    ('hp-victus-16',
     'AMD Ryzen 7 7700HX (8-core, up to 5.0GHz, 32MB Cache)', '16GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4060 8GB GDDR6',
     N'16.1" FHD 1920×1080, 144Hz, 300 nits, IPS',
     'Windows 11 Home', '1× USB-C 3.2, 3× USB-A 3.2, HDMI 2.1, RJ-45, SD card reader, 3.5mm', '2.48 kg', N'70.9Wh, lên đến 8 giờ'),

    ('hp-elitebook-840-g10',
     'Intel Core i7-1355U (10-core, up to 5.0GHz, 12MB Cache)', '16GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Iris Xe Graphics',
     N'14" FHD 1920×1080, 60Hz, 400 nits, IPS, Sure View Reflect chống nhìn trộm',
     'Windows 11 Pro', '2× Thunderbolt 4, 2× USB-A 3.2, HDMI 2.0, SD card reader, RJ-45, 3.5mm', '1.34 kg', N'51Wh, lên đến 10 giờ'),

    ('lenovo-thinkpad-x1-carbon-gen11',
     'Intel Core i7-1365U (10-core, up to 5.2GHz, 12MB Cache)', '16GB LPDDR5 6400MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Iris Xe Graphics',
     N'14" IPS 2.8K 2880×1800, 60Hz, 400 nits, 100% sRGB, Low Blue Light',
     'Windows 11 Pro', '2× Thunderbolt 4, 2× USB-A 3.2, HDMI 2.0b, 3.5mm', '1.12 kg', N'57Wh, lên đến 15 giờ'),

    ('lenovo-legion-5-pro',
     'AMD Ryzen 7 7745HX (8-core, up to 5.4GHz, 32MB Cache)', '16GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4070 8GB GDDR6',
     N'16" QHD+ 2560×1600, 165Hz, 500 nits, 100% sRGB, IPS',
     'Windows 11 Home', '1× Thunderbolt 4, 3× USB-A 3.2, HDMI 2.1, SD card reader, RJ-45, 3.5mm', '2.40 kg', N'80Wh, lên đến 8 giờ'),

    ('lenovo-ideapad-slim-5',
     'AMD Ryzen 5 7530U (6-core, up to 4.5GHz, 16MB Cache)', '16GB DDR4 3200MHz',
     'SSD 512GB NVMe', 'AMD Radeon 610M (integrated)',
     N'14" FHD 1920×1080, 60Hz, 300 nits, IPS',
     'Windows 11 Home', '1× USB-C 3.2, 2× USB-A 3.2, HDMI 1.4, SD card reader, 3.5mm', '1.46 kg', N'56Wh, lên đến 8 giờ'),

    ('asus-rog-strix-g16-2024',
     'Intel Core i9-14900HX (24-core, up to 5.8GHz, 36MB Cache)', '32GB DDR5 4800MHz',
     'SSD 1TB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4070 8GB GDDR6',
     N'16" QHD+ 2560×1600, 240Hz, 500 nits, 100% DCI-P3, IPS',
     'Windows 11 Home', '1× Thunderbolt 4, 1× USB-C 3.2, 2× USB-A 3.2, HDMI 2.1, RJ-45, SD card reader, 3.5mm', '2.50 kg', N'90Wh, lên đến 8 giờ'),

    ('asus-zenbook-14-oled',
     'Intel Core Ultra 7 155H (22-core, up to 4.8GHz, 24MB Cache)', '16GB LPDDR5x 7467MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Arc Graphics',
     N'14" OLED 2.8K 2880×1800, 90Hz, 550 nits, 100% DCI-P3, VESA HDR True Black 500',
     'Windows 11 Home', '2× Thunderbolt 4, 1× USB-A 3.2, HDMI 2.1, SD card reader, 3.5mm', '1.20 kg', N'75Wh, lên đến 15 giờ'),

    ('asus-proart-studiobook-16-oled',
     'Intel Core i9-13980HX (24-core, up to 5.6GHz, 36MB Cache)', '32GB DDR5 4800MHz',
     'SSD 2TB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4070 8GB GDDR6',
     N'16" OLED 3.2K 3200×2000, 60Hz, 550 nits, 100% DCI-P3, cảm ứng, hỗ trợ bút',
     'Windows 11 Pro', '2× Thunderbolt 4, 2× USB-A 3.2, HDMI 2.0b, SD card reader, RJ-45, 3.5mm', '2.40 kg', N'96Wh, lên đến 10 giờ'),

    ('acer-nitro-5-an515-2024',
     'Intel Core i5-13420H (12-core, up to 4.6GHz, 12MB Cache)', '8GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4060 8GB GDDR6',
     N'15.6" FHD 1920×1080, 165Hz, 300 nits, IPS',
     'Windows 11 Home', '1× USB-C 3.2, 3× USB-A 3.2, HDMI 2.1, RJ-45, SD card reader, 3.5mm', '2.30 kg', N'57.5Wh, lên đến 7 giờ'),

    ('acer-swift-3-sf314',
     'AMD Ryzen 5 7530U (6-core, up to 4.5GHz, 16MB Cache)', '8GB LPDDR4X 4266MHz',
     'SSD 512GB NVMe', 'AMD Radeon 610M (integrated)',
     N'14" FHD 1920×1080, 60Hz, 300 nits, IPS',
     'Windows 11 Home', '1× USB-C 3.2, 2× USB-A 3.2, HDMI 2.0, SD card reader, 3.5mm', '1.25 kg', N'56Wh, lên đến 10 giờ'),

    ('msi-katana-15-b13vgk',
     'Intel Core i7-13620H (16-core, up to 4.9GHz, 24MB Cache)', '16GB DDR5 4800MHz',
     'SSD 512GB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4070 8GB GDDR6',
     N'15.6" FHD 1920×1080, 144Hz, 250 nits, IPS',
     'Windows 11 Home', '1× USB-C 3.2, 3× USB-A 3.2, HDMI 2.0b, RJ-45, SD card reader, 3.5mm', '2.20 kg', N'53.5Wh, lên đến 7 giờ'),

    ('razer-blade-15-2024',
     'Intel Core i9-13950HX (24-core, up to 5.6GHz, 36MB Cache)', '16GB DDR5 5600MHz',
     'SSD 1TB NVMe PCIe 4.0', 'NVIDIA GeForce RTX 4070 Ti Super 16GB GDDR6X',
     N'15.6" QHD 2560×1440, 240Hz, 400 nits, 100% DCI-P3, IPS',
     'Windows 11 Home', '2× Thunderbolt 4, 3× USB-A 3.2, HDMI 2.1, SD card reader, 3.5mm', '2.01 kg', N'80Wh, lên đến 8 giờ'),

    ('lg-gram-14-2024',
     'Intel Core Ultra 7 155H (22-core, up to 4.8GHz, 24MB Cache)', '16GB LPDDR5x 7467MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Arc Graphics',
     N'14" IPS WUXGA 1920×1200, 60Hz, 400 nits, 100% DCI-P3',
     'Windows 11 Home', '2× Thunderbolt 4, 2× USB-A 3.2, HDMI 2.0, SD card reader, 3.5mm', '0.99 kg', N'72Wh, lên đến 25 giờ'),

    ('samsung-galaxy-book4-pro-14',
     'Intel Core Ultra 7 155H (22-core, up to 4.8GHz, 24MB Cache)', '16GB LPDDR5x 7467MHz',
     'SSD 512GB NVMe PCIe 4.0', 'Intel Arc Graphics',
     N'14" Dynamic AMOLED 2X 3K 2880×1800, 120Hz, 500 nits, 100% DCI-P3',
     'Windows 11 Home', '2× Thunderbolt 4, 1× USB-A 3.2, HDMI 2.0, SD card reader, 3.5mm', '1.17 kg', N'63Wh, lên đến 22 giờ')
) AS s(Slug, CPU, RAM, Storage, GPU, Screen, OS, Ports, Weight, Battery)
JOIN Products pr ON pr.Slug = s.Slug
WHERE NOT EXISTS (SELECT 1 FROM ProductSpecifications ps WHERE ps.ProductId = pr.Id);

-- ============================================================
-- 5. INVENTORIES
-- Status: 1=InStock, 2=LowStock
-- ============================================================
INSERT INTO Inventories (ProductId, CurrentStock, MinimumStock, ReorderPoint, AverageCost, LastPurchasePrice, LastUpdated, Location, Status)
SELECT pr.Id, inv.CurrentStock, 5, 10, inv.AvgCost, inv.LastPrice, GETUTCDATE(), N'Kho Hà Nội', inv.Status
FROM (VALUES
    ('apple-macbook-pro-14-m3-pro',       12, 47000000, 47000000, 1),
    ('apple-macbook-pro-16-m3-max',        8, 82000000, 82000000, 1),
    ('apple-macbook-air-13-m2',           25, 24000000, 24000000, 1),
    ('apple-macbook-air-15-m3',           18, 31000000, 31000000, 1),
    ('dell-xps-15-9530',                  10, 40000000, 40000000, 1),
    ('dell-xps-13-plus',                  15, 29000000, 29000000, 1),
    ('dell-alienware-m16-r2',              6, 52000000, 52000000, 1),
    ('dell-inspiron-15-3530',             30, 15000000, 15000000, 1),
    ('hp-spectre-x360-14',                14, 34000000, 34000000, 1),
    ('hp-victus-16',                      20, 21000000, 21000000, 1),
    ('hp-elitebook-840-g10',              12, 27000000, 27000000, 1),
    ('lenovo-thinkpad-x1-carbon-gen11',   10, 36000000, 36000000, 1),
    ('lenovo-legion-5-pro',               15, 32000000, 32000000, 1),
    ('lenovo-ideapad-slim-5',             35, 13000000, 13000000, 1),
    ('asus-rog-strix-g16-2024',           12, 37000000, 37000000, 1),
    ('asus-zenbook-14-oled',              20, 19000000, 19000000, 1),
    ('asus-proart-studiobook-16-oled',     7, 48000000, 48000000, 1),
    ('acer-nitro-5-an515-2024',           25, 19000000, 19000000, 1),
    ('acer-swift-3-sf314',                40, 10500000, 10500000, 1),
    ('msi-katana-15-b13vgk',              18, 26000000, 26000000, 1),
    ('razer-blade-15-2024',                5, 68000000, 68000000, 2),
    ('lg-gram-14-2024',                   15, 24000000, 24000000, 1),
    ('samsung-galaxy-book4-pro-14',       15, 29000000, 29000000, 1)
) AS inv(Slug, CurrentStock, AvgCost, LastPrice, Status)
JOIN Products pr ON pr.Slug = inv.Slug
WHERE NOT EXISTS (SELECT 1 FROM Inventories i WHERE i.ProductId = pr.Id);

COMMIT TRANSACTION;

-- Kiểm tra kết quả
PRINT '=== Seed hoàn tất ===';
SELECT 'Brands'               AS [Table], COUNT(*) AS [Count] FROM Brands
UNION ALL
SELECT 'Categories',                      COUNT(*)            FROM Categories       WHERE IsDeleted = 0
UNION ALL
SELECT 'Products',                        COUNT(*)            FROM Products
UNION ALL
SELECT 'ProductSpecifications',           COUNT(*)            FROM ProductSpecifications
UNION ALL
SELECT 'Inventories',                     COUNT(*)            FROM Inventories;

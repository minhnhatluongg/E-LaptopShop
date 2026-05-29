/* ==========================================================
   Seed: ProductAttributes + ProductAttributeValues
   Dùng cho hệ thống biến thể sản phẩm (Variant System).
   Idempotent — WHERE NOT EXISTS.
   ========================================================== */

SET NOCOUNT ON;
BEGIN TRANSACTION;

-- ── 1. ProductAttributes ──────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM ProductAttributes WHERE Slug = 'ram')
    INSERT INTO ProductAttributes (Name, Slug, IsActive) VALUES (N'RAM', 'ram', 1);

IF NOT EXISTS (SELECT 1 FROM ProductAttributes WHERE Slug = 'ssd')
    INSERT INTO ProductAttributes (Name, Slug, IsActive) VALUES (N'SSD', 'ssd', 1);

IF NOT EXISTS (SELECT 1 FROM ProductAttributes WHERE Slug = 'color')
    INSERT INTO ProductAttributes (Name, Slug, IsActive) VALUES (N'Màu sắc', 'color', 1);

IF NOT EXISTS (SELECT 1 FROM ProductAttributes WHERE Slug = 'cpu')
    INSERT INTO ProductAttributes (Name, Slug, IsActive) VALUES (N'CPU', 'cpu', 1);

IF NOT EXISTS (SELECT 1 FROM ProductAttributes WHERE Slug = 'gpu')
    INSERT INTO ProductAttributes (Name, Slug, IsActive) VALUES (N'GPU', 'gpu', 1);

-- ── 2. ProductAttributeValues — RAM ───────────────────────
DECLARE @ramId INT = (SELECT Id FROM ProductAttributes WHERE Slug = 'ram');
INSERT INTO ProductAttributeValues (AttributeId, Value, DisplayOrder)
SELECT @ramId, v, o FROM (VALUES
    (N'8GB',  1), (N'16GB', 2), (N'24GB', 3), (N'32GB', 4),
    (N'36GB', 5), (N'48GB', 6), (N'64GB', 7), (N'96GB', 8)
) AS t(v, o)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductAttributeValues
    WHERE AttributeId = @ramId AND Value = t.v
);

-- ── 3. ProductAttributeValues — SSD ───────────────────────
DECLARE @ssdId INT = (SELECT Id FROM ProductAttributes WHERE Slug = 'ssd');
INSERT INTO ProductAttributeValues (AttributeId, Value, DisplayOrder)
SELECT @ssdId, v, o FROM (VALUES
    (N'256GB',  1), (N'512GB', 2), (N'1TB',  3),
    (N'2TB',    4), (N'4TB',   5)
) AS t(v, o)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductAttributeValues
    WHERE AttributeId = @ssdId AND Value = t.v
);

-- ── 4. ProductAttributeValues — Màu sắc ───────────────────
DECLARE @colorId INT = (SELECT Id FROM ProductAttributes WHERE Slug = 'color');
INSERT INTO ProductAttributeValues (AttributeId, Value, DisplayOrder)
SELECT @colorId, v, o FROM (VALUES
    (N'Space Black',    1), (N'Space Gray',  2), (N'Silver',        3),
    (N'Midnight',       4), (N'Starlight',   5), (N'Sky Blue',      6),
    (N'Natural Titanium',7),(N'Black Titanium',8),(N'White Titanium',9),
    (N'Cosmic Black',  10), (N'Platinum',    11)
) AS t(v, o)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductAttributeValues
    WHERE AttributeId = @colorId AND Value = t.v
);

-- ── 5. ProductAttributeValues — CPU ───────────────────────
DECLARE @cpuId INT = (SELECT Id FROM ProductAttributes WHERE Slug = 'cpu');
INSERT INTO ProductAttributeValues (AttributeId, Value, DisplayOrder)
SELECT @cpuId, v, o FROM (VALUES
    (N'M3',            1), (N'M3 Pro',       2), (N'M3 Max',       3),
    (N'M4',            4), (N'M4 Pro',       5), (N'M4 Max',       6),
    (N'Core Ultra 5',  7), (N'Core Ultra 7', 8), (N'Core Ultra 9', 9),
    (N'Ryzen 5',      10), (N'Ryzen 7',     11), (N'Ryzen 9',     12),
    (N'Snapdragon X Plus', 13), (N'Snapdragon X Elite', 14)
) AS t(v, o)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductAttributeValues
    WHERE AttributeId = @cpuId AND Value = t.v
);

-- ── 6. ProductAttributeValues — GPU ───────────────────────
DECLARE @gpuId INT = (SELECT Id FROM ProductAttributes WHERE Slug = 'gpu');
INSERT INTO ProductAttributeValues (AttributeId, Value, DisplayOrder)
SELECT @gpuId, v, o FROM (VALUES
    (N'RTX 4050', 1), (N'RTX 4060',   2), (N'RTX 4070',  3),
    (N'RTX 4080', 4), (N'RTX 4090',   5), (N'RTX 5080',  6),
    (N'RTX 5090', 7), (N'RX 7600M',  8), (N'RX 7700S',  9),
    (N'Arc',      10),(N'Integrated', 11)
) AS t(v, o)
WHERE NOT EXISTS (
    SELECT 1 FROM ProductAttributeValues
    WHERE AttributeId = @gpuId AND Value = t.v
);

COMMIT TRANSACTION;

PRINT '=== 13_seed_attributes hoàn tất ===';
SELECT a.Name AS Attribute, COUNT(v.Id) AS ValueCount
FROM ProductAttributes a
LEFT JOIN ProductAttributeValues v ON v.AttributeId = a.Id
GROUP BY a.Id, a.Name
ORDER BY a.Name;

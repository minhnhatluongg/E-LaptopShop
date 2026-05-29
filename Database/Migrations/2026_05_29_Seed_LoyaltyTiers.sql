-- =============================================
-- Seed dữ liệu LoyaltyTiers (4 tier cơ bản).
-- Tiêu chí thăng hạng dựa trên LifetimeSpend (tổng chi tiêu trên các đơn đã giao).
--
-- | Tier      | MinSpend (đ)  | Discount | Hệ số điểm  |
-- |-----------|---------------|----------|-------------|
-- | Đồng      |          0    | 0%       | x1.0        |
-- | Bạc       |  5.000.000    | 2%       | x1.2        |
-- | Vàng      | 30.000.000    | 5%       | x1.5        |
-- | Bạch Kim  |100.000.000    | 8%       | x2.0        |
--
-- Quy đổi điểm: 1.000 đ chi tiêu → 1 điểm × PointsMultiplier của tier hiện tại.
-- Idempotent — chỉ insert nếu chưa có Name.
-- =============================================

IF NOT EXISTS (SELECT 1 FROM dbo.LoyaltyTiers WHERE Name = N'Đồng')
    INSERT INTO dbo.LoyaltyTiers (Name, MinSpend, DiscountPercent, PointsMultiplier, IsActive)
    VALUES (N'Đồng', 0, 0, 1.0, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.LoyaltyTiers WHERE Name = N'Bạc')
    INSERT INTO dbo.LoyaltyTiers (Name, MinSpend, DiscountPercent, PointsMultiplier, IsActive)
    VALUES (N'Bạc', 5000000, 2, 1.2, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.LoyaltyTiers WHERE Name = N'Vàng')
    INSERT INTO dbo.LoyaltyTiers (Name, MinSpend, DiscountPercent, PointsMultiplier, IsActive)
    VALUES (N'Vàng', 30000000, 5, 1.5, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.LoyaltyTiers WHERE Name = N'Bạch Kim')
    INSERT INTO dbo.LoyaltyTiers (Name, MinSpend, DiscountPercent, PointsMultiplier, IsActive)
    VALUES (N'Bạch Kim', 100000000, 8, 2.0, 1);

PRINT 'Seed LoyaltyTiers OK.';
GO

-- Coupon mẫu WELCOME10: tặng cho người mua hàng lần đầu (-10%, tối đa 500.000đ)
IF NOT EXISTS (SELECT 1 FROM dbo.Coupons WHERE Code = 'WELCOME10')
    INSERT INTO dbo.Coupons
        (Code, Description, DiscountType, DiscountValue, MinOrderAmount,
         MaxDiscountAmount, UsageLimit, UsageLimitPerUser, UsedCount,
         StartsAt, EndsAt, IsActive, CreatedAt, CreatedBy)
    VALUES
        ('WELCOME10', N'Tặng 10% cho đơn đầu tiên (tối đa 500.000đ)',
         'Percent', 10, 0, 500000, NULL, 1, 0,
         GETDATE(), DATEADD(YEAR, 1, GETDATE()), 1, GETDATE(), 'system');

PRINT 'Seed Coupon WELCOME10 OK.';
GO

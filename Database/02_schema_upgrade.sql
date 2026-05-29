/* =============================================================
   E-LaptopShop — Schema Upgrade (Phase 2)
   Mục tiêu:  Bổ sung các bảng nâng cao để làm "điểm nhấn"
   Target: Azure SQL Database
   Prereq: chạy 01_schema_current.sql trước.

   Các nhóm bổ sung:
     A. Marketing       — Coupons, ProductTags, ProductTagMap, Wishlists
     B. Customer 360    — Notifications, AuditLogs, ActivityLogs
     C. Loyalty         — LoyaltyTiers, UserLoyalty, PointTransactions
     D. After-sales     — ReturnRequests, RefundTransactions
     E. CMS             — Banners, Posts (Blog), ContactMessages
     F. Variants        — ProductVariants, ProductAttributes (laptop khác cấu hình)
     G. Reporting layer — Views & indexed helpers cho dashboard
   ============================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* =============================================================
   A. MARKETING
   ============================================================= */

/* A.1 Coupons — quản lý mã giảm giá theo % hoặc số tiền */
IF OBJECT_ID(N'dbo.Coupons', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Coupons
    (
        Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Coupons PRIMARY KEY,
        Code             NVARCHAR(50)  NOT NULL,
        [Description]    NVARCHAR(255) NULL,
        DiscountType     NVARCHAR(20)  NOT NULL CONSTRAINT DF_Coupons_DiscountType DEFAULT(N'PERCENT'), -- PERCENT | AMOUNT
        DiscountValue    DECIMAL(18,2) NOT NULL,
        MinOrderAmount   DECIMAL(18,2) NOT NULL CONSTRAINT DF_Coupons_MinOrder DEFAULT(0),
        MaxDiscountAmount DECIMAL(18,2) NULL,
        UsageLimit       INT           NULL,                       -- tổng số lần được sử dụng
        UsageLimitPerUser INT          NULL,                       -- tối đa / user
        UsedCount        INT           NOT NULL CONSTRAINT DF_Coupons_UsedCount DEFAULT(0),
        StartsAt         DATETIME2     NOT NULL CONSTRAINT DF_Coupons_StartsAt DEFAULT(SYSUTCDATETIME()),
        EndsAt           DATETIME2     NULL,
        IsActive         BIT           NOT NULL CONSTRAINT DF_Coupons_IsActive DEFAULT(1),
        CreatedAt        DATETIME2     NOT NULL CONSTRAINT DF_Coupons_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CreatedBy        NVARCHAR(100) NULL,
        CONSTRAINT CK_Coupons_DiscountType CHECK (DiscountType IN (N'PERCENT', N'AMOUNT')),
        CONSTRAINT CK_Coupons_Value CHECK (DiscountValue > 0)
    );

    CREATE UNIQUE INDEX UX_Coupons_Code ON dbo.Coupons(Code);
END
GO

/* A.2 CouponUsages — log mỗi lần dùng coupon (1 user dùng nhiều lần) */
IF OBJECT_ID(N'dbo.CouponUsages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CouponUsages
    (
        Id        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CouponUsages PRIMARY KEY,
        CouponId  INT          NOT NULL,
        UserId    INT          NULL,
        OrderId   INT          NULL,
        AmountSaved DECIMAL(18,2) NOT NULL,
        UsedAt    DATETIME2    NOT NULL CONSTRAINT DF_CouponUsages_UsedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_CouponUsages_Coupons FOREIGN KEY (CouponId) REFERENCES dbo.Coupons(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CouponUsages_Users   FOREIGN KEY (UserId)   REFERENCES dbo.Users(Id),
        CONSTRAINT FK_CouponUsages_Orders  FOREIGN KEY (OrderId)  REFERENCES dbo.Orders(Id)
    );

    CREATE INDEX IX_CouponUsages_User ON dbo.CouponUsages(UserId);
END
GO

/* A.3 Product Tags (nhãn linh hoạt: "gaming", "ultrabook", "sale-2026"...) */
IF OBJECT_ID(N'dbo.ProductTags', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductTags
    (
        Id       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductTags PRIMARY KEY,
        Name     NVARCHAR(80) NOT NULL,
        Slug     VARCHAR(120) NOT NULL,
        IsActive BIT          NOT NULL CONSTRAINT DF_ProductTags_IsActive DEFAULT(1)
    );

    CREATE UNIQUE INDEX UX_ProductTags_Slug ON dbo.ProductTags(Slug);
END
GO

IF OBJECT_ID(N'dbo.ProductTagMap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductTagMap
    (
        ProductId INT NOT NULL,
        TagId     INT NOT NULL,
        CONSTRAINT PK_ProductTagMap PRIMARY KEY (ProductId, TagId),
        CONSTRAINT FK_ProductTagMap_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)     ON DELETE CASCADE,
        CONSTRAINT FK_ProductTagMap_Tags     FOREIGN KEY (TagId)     REFERENCES dbo.ProductTags(Id) ON DELETE CASCADE
    );
END
GO

/* A.4 Wishlists */
IF OBJECT_ID(N'dbo.Wishlists', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Wishlists
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Wishlists PRIMARY KEY,
        UserId    INT NOT NULL,
        ProductId INT NOT NULL,
        AddedAt   DATETIME2 NOT NULL CONSTRAINT DF_Wishlists_AddedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_Wishlists_Users    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id)    ON DELETE CASCADE,
        CONSTRAINT FK_Wishlists_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_Wishlists_User_Product ON dbo.Wishlists(UserId, ProductId);
END
GO

/* =============================================================
   B. CUSTOMER 360 (notifications, audit log, activity log)
   ============================================================= */

IF OBJECT_ID(N'dbo.Notifications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications
    (
        Id        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notifications PRIMARY KEY,
        UserId    INT          NULL,
        Title     NVARCHAR(150) NOT NULL,
        Body      NVARCHAR(MAX) NULL,
        [Type]    NVARCHAR(50) NOT NULL CONSTRAINT DF_Notifications_Type DEFAULT(N'INFO'),  -- INFO|ORDER|PROMO|SYSTEM
        Url       NVARCHAR(255) NULL,
        IsRead    BIT          NOT NULL CONSTRAINT DF_Notifications_IsRead DEFAULT(0),
        ReadAt    DATETIME2    NULL,
        CreatedAt DATETIME2    NOT NULL CONSTRAINT DF_Notifications_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Notifications_User_Unread
        ON dbo.Notifications(UserId, IsRead) INCLUDE (CreatedAt);
END
GO

/* AuditLogs: ai làm gì với entity nào (dùng cho admin trace) */
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs
    (
        Id        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLogs PRIMARY KEY,
        UserId    INT          NULL,
        EntityName NVARCHAR(100) NOT NULL,           -- Products, Orders, ...
        EntityId  NVARCHAR(50) NULL,                 -- để string để chứa GUID/INT
        [Action]  NVARCHAR(20) NOT NULL,             -- CREATE|UPDATE|DELETE
        OldValues NVARCHAR(MAX) NULL,                -- JSON snapshot trước
        NewValues NVARCHAR(MAX) NULL,                -- JSON snapshot sau
        IpAddress NVARCHAR(45)  NULL,
        UserAgent NVARCHAR(255) NULL,
        CreatedAt DATETIME2     NOT NULL CONSTRAINT DF_AuditLogs_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );

    CREATE INDEX IX_AuditLogs_Entity      ON dbo.AuditLogs(EntityName, EntityId);
    CREATE INDEX IX_AuditLogs_CreatedAt   ON dbo.AuditLogs(CreatedAt);
END
GO

/* ActivityLogs: login, view product, search... — phục vụ recommend/analytic */
IF OBJECT_ID(N'dbo.ActivityLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ActivityLogs
    (
        Id        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ActivityLogs PRIMARY KEY,
        UserId    INT          NULL,
        SessionId NVARCHAR(64) NULL,
        EventType NVARCHAR(50) NOT NULL,             -- LOGIN | VIEW_PRODUCT | ADD_TO_CART | CHECKOUT | SEARCH
        Metadata  NVARCHAR(MAX) NULL,                -- JSON: { productId, searchTerm, ... }
        IpAddress NVARCHAR(45)  NULL,
        CreatedAt DATETIME2     NOT NULL CONSTRAINT DF_ActivityLogs_CreatedAt DEFAULT(SYSUTCDATETIME())
    );

    CREATE INDEX IX_ActivityLogs_User_Event ON dbo.ActivityLogs(UserId, EventType, CreatedAt DESC);
END
GO

/* =============================================================
   C. LOYALTY  (membership tier + point)
   ============================================================= */

IF OBJECT_ID(N'dbo.LoyaltyTiers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoyaltyTiers
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LoyaltyTiers PRIMARY KEY,
        Name            NVARCHAR(50) NOT NULL,        -- Bronze | Silver | Gold | Platinum
        MinSpend        DECIMAL(18,2) NOT NULL,
        DiscountPercent DECIMAL(5,2)  NOT NULL CONSTRAINT DF_LoyaltyTiers_Discount DEFAULT(0),
        PointsMultiplier DECIMAL(5,2) NOT NULL CONSTRAINT DF_LoyaltyTiers_Mult DEFAULT(1),
        IsActive        BIT NOT NULL CONSTRAINT DF_LoyaltyTiers_IsActive DEFAULT(1)
    );

    CREATE UNIQUE INDEX UX_LoyaltyTiers_Name ON dbo.LoyaltyTiers(Name);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LoyaltyTiers)
BEGIN
    INSERT INTO dbo.LoyaltyTiers (Name, MinSpend, DiscountPercent, PointsMultiplier) VALUES
        (N'Bronze',   0,        0,  1.00),
        (N'Silver',   10000000, 2,  1.25),
        (N'Gold',     30000000, 5,  1.50),
        (N'Platinum', 80000000, 10, 2.00);
END
GO

IF OBJECT_ID(N'dbo.UserLoyalty', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserLoyalty
    (
        UserId        INT NOT NULL CONSTRAINT PK_UserLoyalty PRIMARY KEY,
        TierId        INT NOT NULL,
        TotalPoints   INT NOT NULL CONSTRAINT DF_UserLoyalty_TotalPoints DEFAULT(0),
        LifetimeSpend DECIMAL(18,2) NOT NULL CONSTRAINT DF_UserLoyalty_Lifetime DEFAULT(0),
        UpdatedAt     DATETIME2 NOT NULL CONSTRAINT DF_UserLoyalty_UpdatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_UserLoyalty_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)         ON DELETE CASCADE,
        CONSTRAINT FK_UserLoyalty_Tier  FOREIGN KEY (TierId) REFERENCES dbo.LoyaltyTiers(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.PointTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PointTransactions
    (
        Id        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PointTransactions PRIMARY KEY,
        UserId    INT          NOT NULL,
        OrderId   INT          NULL,
        Points    INT          NOT NULL,                                 -- + earn, - redeem
        Reason    NVARCHAR(100) NOT NULL,                                -- ORDER | REDEEM | ADJUST | EXPIRE
        CreatedAt DATETIME2    NOT NULL CONSTRAINT DF_PointTransactions_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_PointTransactions_Users  FOREIGN KEY (UserId)  REFERENCES dbo.Users(Id)  ON DELETE CASCADE,
        CONSTRAINT FK_PointTransactions_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
    );
END
GO

/* =============================================================
   D. AFTER-SALES (return + refund)
   ============================================================= */

IF OBJECT_ID(N'dbo.ReturnRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReturnRequests
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ReturnRequests PRIMARY KEY,
        OrderId       INT          NOT NULL,
        OrderItemId   INT          NULL,
        UserId        INT          NULL,
        Reason        NVARCHAR(255) NOT NULL,
        Status        NVARCHAR(50)  NOT NULL CONSTRAINT DF_ReturnRequests_Status DEFAULT(N'Pending'),
        RequestedAt   DATETIME2    NOT NULL CONSTRAINT DF_ReturnRequests_RequestedAt DEFAULT(SYSUTCDATETIME()),
        ResolvedAt    DATETIME2    NULL,
        ResolvedBy    NVARCHAR(100) NULL,
        Notes         NVARCHAR(MAX) NULL,
        CONSTRAINT FK_ReturnRequests_Orders     FOREIGN KEY (OrderId)     REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_ReturnRequests_OrderItems FOREIGN KEY (OrderItemId) REFERENCES dbo.OrderItems(Id),
        CONSTRAINT FK_ReturnRequests_Users      FOREIGN KEY (UserId)      REFERENCES dbo.Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.RefundTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefundTransactions
    (
        Id                    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RefundTransactions PRIMARY KEY,
        OrderId               INT           NOT NULL,
        PaymentTransactionId  INT           NULL,
        ReturnRequestId       INT           NULL,
        Amount                DECIMAL(18,2) NOT NULL,
        Status                NVARCHAR(50)  NOT NULL CONSTRAINT DF_RefundTransactions_Status DEFAULT(N'Pending'),
        RefundMethod          NVARCHAR(50)  NULL,
        ProcessedAt           DATETIME2     NULL,
        Notes                 NVARCHAR(500) NULL,
        CreatedAt             DATETIME2     NOT NULL CONSTRAINT DF_RefundTransactions_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_RefundTransactions_Orders   FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_RefundTransactions_Payment  FOREIGN KEY (PaymentTransactionId) REFERENCES dbo.PaymentTransactions(Id),
        CONSTRAINT FK_RefundTransactions_Return   FOREIGN KEY (ReturnRequestId)      REFERENCES dbo.ReturnRequests(Id)
    );
END
GO

/* =============================================================
   E. CMS (banners + blog posts + contact messages)
   ============================================================= */

IF OBJECT_ID(N'dbo.Banners', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Banners
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Banners PRIMARY KEY,
        Title        NVARCHAR(150) NOT NULL,
        Subtitle     NVARCHAR(255) NULL,
        ImageUrl     NVARCHAR(500) NOT NULL,
        LinkUrl      NVARCHAR(500) NULL,
        Position     NVARCHAR(50)  NOT NULL CONSTRAINT DF_Banners_Position DEFAULT(N'HOMEPAGE_TOP'),
        DisplayOrder INT           NOT NULL CONSTRAINT DF_Banners_DisplayOrder DEFAULT(0),
        StartsAt     DATETIME2     NULL,
        EndsAt       DATETIME2     NULL,
        IsActive     BIT           NOT NULL CONSTRAINT DF_Banners_IsActive DEFAULT(1),
        CreatedAt    DATETIME2     NOT NULL CONSTRAINT DF_Banners_CreatedAt DEFAULT(SYSUTCDATETIME())
    );
END
GO

IF OBJECT_ID(N'dbo.Posts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Posts
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Posts PRIMARY KEY,
        AuthorUserId  INT           NULL,
        Title         NVARCHAR(200) NOT NULL,
        Slug          VARCHAR(220)  NOT NULL,
        Excerpt       NVARCHAR(500) NULL,
        Content       NVARCHAR(MAX) NULL,
        CoverImageUrl NVARCHAR(500) NULL,
        Status        NVARCHAR(20)  NOT NULL CONSTRAINT DF_Posts_Status DEFAULT(N'DRAFT'), -- DRAFT|PUBLISHED|ARCHIVED
        PublishedAt   DATETIME2     NULL,
        ViewCount     INT           NOT NULL CONSTRAINT DF_Posts_ViewCount DEFAULT(0),
        CreatedAt     DATETIME2     NOT NULL CONSTRAINT DF_Posts_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt     DATETIME2     NULL,
        CONSTRAINT FK_Posts_Users FOREIGN KEY (AuthorUserId) REFERENCES dbo.Users(Id)
    );

    CREATE UNIQUE INDEX UX_Posts_Slug ON dbo.Posts(Slug);
    CREATE INDEX IX_Posts_Status_Published ON dbo.Posts(Status, PublishedAt);
END
GO

IF OBJECT_ID(N'dbo.ContactMessages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactMessages
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ContactMessages PRIMARY KEY,
        FullName  NVARCHAR(100) NOT NULL,
        Email     NVARCHAR(100) NOT NULL,
        Phone     NVARCHAR(20)  NULL,
        Subject   NVARCHAR(200) NULL,
        Message   NVARCHAR(MAX) NOT NULL,
        IsHandled BIT NOT NULL CONSTRAINT DF_ContactMessages_IsHandled DEFAULT(0),
        HandledBy NVARCHAR(100) NULL,
        HandledAt DATETIME2     NULL,
        CreatedAt DATETIME2     NOT NULL CONSTRAINT DF_ContactMessages_CreatedAt DEFAULT(SYSUTCDATETIME())
    );
END
GO

/* =============================================================
   F. PRODUCT VARIANTS (RAM 16/32, SSD 512/1TB...)
      Đây là phần TĂNG ĐỘ PHỨC TẠP rõ nhất cho 1 laptop shop.
   ============================================================= */

IF OBJECT_ID(N'dbo.ProductAttributes', N'U') IS NULL
BEGIN
    /* Thuộc tính có thể chọn cho mỗi product: "RAM", "Storage", "Color" ... */
    CREATE TABLE dbo.ProductAttributes
    (
        Id       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductAttributes PRIMARY KEY,
        Name     NVARCHAR(50) NOT NULL,  -- ví dụ: 'RAM'
        Slug     VARCHAR(80)  NOT NULL,
        IsActive BIT          NOT NULL CONSTRAINT DF_ProductAttributes_IsActive DEFAULT(1)
    );
    CREATE UNIQUE INDEX UX_ProductAttributes_Slug ON dbo.ProductAttributes(Slug);
END
GO

IF OBJECT_ID(N'dbo.ProductAttributeValues', N'U') IS NULL
BEGIN
    /* Giá trị cụ thể: '16GB', '32GB', '512GB SSD', ... */
    CREATE TABLE dbo.ProductAttributeValues
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductAttributeValues PRIMARY KEY,
        AttributeId INT NOT NULL,
        [Value]     NVARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_PAV_DisplayOrder DEFAULT(0),
        CONSTRAINT FK_PAV_Attributes FOREIGN KEY (AttributeId) REFERENCES dbo.ProductAttributes(Id) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX UX_PAV_Attribute_Value ON dbo.ProductAttributeValues(AttributeId, [Value]);
END
GO

IF OBJECT_ID(N'dbo.ProductVariants', N'U') IS NULL
BEGIN
    /* Mỗi variant = 1 SKU cụ thể của 1 product */
    CREATE TABLE dbo.ProductVariants
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductVariants PRIMARY KEY,
        ProductId     INT NOT NULL,
        SKU           NVARCHAR(100) NOT NULL,
        Price         DECIMAL(18,2) NOT NULL,
        CompareAtPrice DECIMAL(18,2) NULL,    -- giá gạch ngang
        CostPrice     DECIMAL(18,2) NULL,
        StockQuantity INT NOT NULL CONSTRAINT DF_ProductVariants_Stock DEFAULT(0),
        Barcode       NVARCHAR(80) NULL,
        IsActive      BIT NOT NULL CONSTRAINT DF_ProductVariants_IsActive DEFAULT(1),
        CreatedAt     DATETIME2 NOT NULL CONSTRAINT DF_ProductVariants_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_ProductVariants_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_ProductVariants_SKU ON dbo.ProductVariants(SKU);
    CREATE INDEX IX_ProductVariants_ProductId ON dbo.ProductVariants(ProductId);
END
GO

IF OBJECT_ID(N'dbo.ProductVariantValueMap', N'U') IS NULL
BEGIN
    /* Liên kết variant ↔ các giá trị thuộc tính */
    CREATE TABLE dbo.ProductVariantValueMap
    (
        VariantId        INT NOT NULL,
        AttributeValueId INT NOT NULL,
        CONSTRAINT PK_ProductVariantValueMap PRIMARY KEY (VariantId, AttributeValueId),
        CONSTRAINT FK_PVVM_Variants FOREIGN KEY (VariantId)        REFERENCES dbo.ProductVariants(Id)        ON DELETE CASCADE,
        CONSTRAINT FK_PVVM_Values   FOREIGN KEY (AttributeValueId) REFERENCES dbo.ProductAttributeValues(Id)
    );
END
GO

/* =============================================================
   G. REPORTING VIEWS (đẹp cho dashboard / portfolio demo)
   ============================================================= */

/* Doanh thu theo ngày */
IF OBJECT_ID(N'dbo.vw_DailyRevenue', N'V') IS NOT NULL
    DROP VIEW dbo.vw_DailyRevenue;
GO
CREATE VIEW dbo.vw_DailyRevenue AS
SELECT
    CAST(OrderDate AS DATE)              AS [Date],
    COUNT(*)                              AS OrderCount,
    SUM(TotalAmount)                      AS Revenue,
    SUM(CASE WHEN IsPaid = 1 THEN TotalAmount ELSE 0 END) AS PaidRevenue
FROM dbo.Orders
WHERE [Status] <> N'Cancelled'
GROUP BY CAST(OrderDate AS DATE);
GO

/* Top sản phẩm bán chạy */
IF OBJECT_ID(N'dbo.vw_TopProducts', N'V') IS NOT NULL
    DROP VIEW dbo.vw_TopProducts;
GO
CREATE VIEW dbo.vw_TopProducts AS
SELECT
    p.Id        AS ProductId,
    p.Name      AS ProductName,
    SUM(oi.Quantity)  AS TotalSold,
    SUM(oi.SubTotal)  AS Revenue
FROM dbo.OrderItems oi
JOIN dbo.Products  p ON p.Id = oi.ProductId
JOIN dbo.Orders    o ON o.Id = oi.OrderId
WHERE o.[Status] <> N'Cancelled'
GROUP BY p.Id, p.Name;
GO

/* Tồn kho cảnh báo */
IF OBJECT_ID(N'dbo.vw_LowStockAlert', N'V') IS NOT NULL
    DROP VIEW dbo.vw_LowStockAlert;
GO
CREATE VIEW dbo.vw_LowStockAlert AS
SELECT
    i.Id            AS InventoryId,
    p.Id            AS ProductId,
    p.Name          AS ProductName,
    i.CurrentStock,
    i.MinimumStock,
    i.ReorderPoint,
    i.[Location]
FROM dbo.Inventories i
JOIN dbo.Products p ON p.Id = i.ProductId
WHERE i.CurrentStock <= i.ReorderPoint;
GO

PRINT N'>>> Done: upgrade schema deployed (Phase 2 — marketing, loyalty, variants, CMS, reports).';
GO

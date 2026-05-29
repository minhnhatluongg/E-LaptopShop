/* =============================================================
   E-LaptopShop — Current Schema (Azure SQL compatible)
   Generated from EF Core entities + ApplicationDbContext
   Target: Azure SQL Database (free tier: free-sql-db-9719142)
   Compatibility: SQL Server 2019+ / Azure SQL
   -------------------------------------------------------------
   Cách dùng:
     1. Tạo Azure SQL DB rỗng tên LaptopStoreDB.
     2. Mở Query editor trên Azure portal hoặc SSMS / Azure Data Studio.
     3. Chạy nguyên file này trên database LaptopStoreDB.
     4. Sau đó chạy 02_schema_upgrade.sql nếu muốn các bảng nâng cao.
   ============================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* -------------------------------------------------------------
   1. Roles
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Name      NVARCHAR(50)  NOT NULL,
        Code      NVARCHAR(50)  NOT NULL CONSTRAINT DF_Roles_Code DEFAULT(N''),
        IsActive  BIT           NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT(1)
    );
END
GO

/* Seed roles (giữ Id để khớp HasData trong EF) */
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id IN (4,5,6,7,8,9,10,11,12,13))
BEGIN
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT INTO dbo.Roles (Id, Name, Code, IsActive) VALUES
        (4,  N'SUPER ADMIN', N'SUPER_ADMIN', 1),
        (5,  N'Sales',       N'SALES',       1),
        (6,  N'Customer',    N'CUSTOMER',    1),
        (7,  N'Admin',       N'ADMIN',       1),
        (8,  N'Manager',     N'MANAGER',     1),
        (9,  N'Warehouse',   N'WAREHOUSE',   1),
        (10, N'Support',     N'SUPPORT',     1),
        (11, N'Moderator',   N'MODERATOR',   1),
        (12, N'VIP',         N'VIP',         1),
        (13, N'Partner',     N'PARTNER',     1);
    SET IDENTITY_INSERT dbo.Roles OFF;
END
GO

/* -------------------------------------------------------------
   2. Users
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id                       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        FirstName                NVARCHAR(50)  NOT NULL,
        LastName                 NVARCHAR(50)  NOT NULL,
        Email                    NVARCHAR(100) NOT NULL,
        PasswordHash             NVARCHAR(255) NULL,
        Phone                    NVARCHAR(20)  NULL,
        AvatarUrl                NVARCHAR(255) NULL,
        Token                    NVARCHAR(255) NULL,
        RefreshToken             NVARCHAR(512) NULL,
        RefreshTokenExpiryTime   DATETIME2     NULL,
        LastLoginAt              DATETIME2     NULL,
        LoginAttempts            INT           NOT NULL CONSTRAINT DF_Users_LoginAttempts DEFAULT(0),
        IsLocked                 BIT           NOT NULL CONSTRAINT DF_Users_IsLocked DEFAULT(0),
        LockedUntil              DATETIME2     NULL,
        RoleId                   INT           NOT NULL,
        IsActive                 BIT           NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT(1),
        EmailConfirmed           BIT           NOT NULL CONSTRAINT DF_Users_EmailConfirmed DEFAULT(0),
        VerificationToken        NVARCHAR(100) NULL,
        Gender                   NVARCHAR(50)  NULL,
        DateOfBirth              DATETIME2     NULL,
        CreatedAt                DATETIME      NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt                DATETIME      NULL,
        CreatedBy                NVARCHAR(100) NULL,
        UpdatedBy                NVARCHAR(100) NULL,
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );

    CREATE UNIQUE INDEX UQ__Users__A9D10534BCF0AE75 ON dbo.Users(Email);
    CREATE INDEX IX_Users_RoleId        ON dbo.Users(RoleId);
    CREATE INDEX IX_Users_IsActive      ON dbo.Users(IsActive);
    CREATE INDEX IX_Users_LastLoginAt   ON dbo.Users(LastLoginAt);
END
GO

/* -------------------------------------------------------------
   3. UserAddresses
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.UserAddresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserAddresses
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserAddresses PRIMARY KEY,
        UserId       INT            NULL,
        FullName     NVARCHAR(100)  NULL,
        Phone        NVARCHAR(20)   NULL,
        AddressLine  NVARCHAR(255)  NULL,
        City         NVARCHAR(100)  NULL,
        District     NVARCHAR(100)  NULL,
        Ward         NVARCHAR(100)  NULL,
        IsDefault    BIT            NOT NULL CONSTRAINT DF_UserAddresses_IsDefault DEFAULT(0),
        CountryCode  NVARCHAR(50)   NOT NULL CONSTRAINT DF_UserAddresses_CountryCode DEFAULT(N'VN'),
        PostalCode   NVARCHAR(30)   NULL,
        CreatedAt    DATETIME2      NOT NULL CONSTRAINT DF_UserAddresses_CreatedAt DEFAULT(SYSUTCDATETIME()),
        DeletedAt    DATETIMEOFFSET NULL,
        IsDeleted    BIT            NOT NULL CONSTRAINT DF_UserAddresses_IsDeleted DEFAULT(0),
        UpdatedAt    DATETIMEOFFSET NULL,
        CONSTRAINT FK_UserAddresses_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );

    /* Mỗi user chỉ có 1 địa chỉ mặc định */
    CREATE UNIQUE INDEX UX_UserAddresses_User_Default
        ON dbo.UserAddresses(UserId, IsDefault)
        WHERE IsDefault = 1;
END
GO

/* -------------------------------------------------------------
   4. Brands
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Brands', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Brands
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Brands PRIMARY KEY,
        Name         NVARCHAR(100) NOT NULL,
        Description  NVARCHAR(MAX) NULL,
        Slug         VARCHAR(200)  NULL,
        IsActive     BIT           NOT NULL CONSTRAINT DF_Brands_IsActive DEFAULT(1),
        CreatedAt    DATETIME      NOT NULL CONSTRAINT DF_Brands_CreatedAt DEFAULT(GETUTCDATE())
    );

    CREATE UNIQUE INDEX UX_Brands_Slug ON dbo.Brands(Slug) WHERE Slug IS NOT NULL;
END
GO

/* -------------------------------------------------------------
   5. Categories  (self-referencing tree, soft-delete, rowversion)
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
        Name            NVARCHAR(100)  NOT NULL,
        Slug            VARCHAR(200)   NOT NULL,
        Description     NVARCHAR(255)  NULL,
        ParentId        INT            NULL,
        IsActive        BIT            NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT(1),
        DisplayOrder    INT            NOT NULL CONSTRAINT DF_Categories_DisplayOrder DEFAULT(0),
        ImageFileId     BIGINT         NULL,
        MetaTitle       NVARCHAR(150)  NULL,
        MetaDescription NVARCHAR(300)  NULL,
        MetaKeywords    NVARCHAR(200)  NULL,
        CreatedAt       DATETIME2      NOT NULL CONSTRAINT DF_Categories_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt       DATETIME2      NULL,
        CreatedBy       NVARCHAR(64)   NULL,
        UpdatedBy       NVARCHAR(64)   NULL,
        IsDeleted       BIT            NOT NULL CONSTRAINT DF_Categories_IsDeleted DEFAULT(0),
        DeletedAt       DATETIME2      NULL,
        RowVersion      ROWVERSION     NOT NULL,
        CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentId)
            REFERENCES dbo.Categories(Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_Categories_Parent_Display
        ON dbo.Categories(ParentId, DisplayOrder);

    CREATE UNIQUE INDEX UX_Categories_Slug
        ON dbo.Categories(Slug) WHERE IsDeleted = 0;

    CREATE UNIQUE INDEX UX_Categories_Parent_Name
        ON dbo.Categories(ParentId, Name) WHERE IsDeleted = 0;
END
GO

/* -------------------------------------------------------------
   6. Products
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        Name        NVARCHAR(150)  NOT NULL,
        Slug        VARCHAR(200)   NOT NULL,
        Description NVARCHAR(MAX)  NULL,
        Price       DECIMAL(18,2)  NOT NULL,
        Discount    DECIMAL(5,2)   NULL CONSTRAINT DF_Products_Discount DEFAULT(0),
        InStock     INT            NULL CONSTRAINT DF_Products_InStock DEFAULT(0),
        CategoryId  INT            NULL,
        IsActive    BIT            NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT(1),
        BrandId     INT            NULL,
        CreatedAt   DATETIME       NULL CONSTRAINT DF_Products_CreatedAt DEFAULT(GETUTCDATE()),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
        CONSTRAINT FK_Products_Brands     FOREIGN KEY (BrandId)    REFERENCES dbo.Brands(Id)
    );

    CREATE UNIQUE INDEX UX_Products_Slug ON dbo.Products(Slug);
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
    CREATE INDEX IX_Products_BrandId    ON dbo.Products(BrandId);
    CREATE INDEX IX_Products_IsActive   ON dbo.Products(IsActive);
END
GO

/* -------------------------------------------------------------
   7. SysFile (storage metadata)
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.SysFile', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SysFile
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SysFile PRIMARY KEY,
        FileName    NVARCHAR(255) NOT NULL,
        FilePath    NVARCHAR(255) NOT NULL,
        FileUrl     NVARCHAR(255) NOT NULL,
        FileType    NVARCHAR(50)  NOT NULL,
        FileSize    BIGINT        NOT NULL CONSTRAINT DF_SysFile_FileSize DEFAULT(0),
        StorageType NVARCHAR(100) NOT NULL CONSTRAINT DF_SysFile_StorageType DEFAULT(N'local'),
        UploadedAt  DATETIME      NOT NULL CONSTRAINT DF_SysFile_UploadedAt DEFAULT(GETDATE()),
        UploadedBy  NVARCHAR(50)  NULL,
        IsActive    BIT           NOT NULL CONSTRAINT DF_SysFile_IsActive DEFAULT(1)
    );
END
GO

/* -------------------------------------------------------------
   8. ProductImages
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ProductImages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductImages
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductImages PRIMARY KEY,
        ProductId    INT          NOT NULL,
        SysFileId    INT          NULL,
        ImageUrl     NVARCHAR(255) NOT NULL,
        IsMain       BIT          NOT NULL CONSTRAINT DF_ProductImages_IsMain DEFAULT(0),
        FileType     NVARCHAR(50) NOT NULL,
        FileSize     BIGINT       NOT NULL CONSTRAINT DF_ProductImages_FileSize DEFAULT(0),
        DisplayOrder INT          NOT NULL CONSTRAINT DF_ProductImages_DisplayOrder DEFAULT(0),
        AltText      NVARCHAR(255) NULL,
        Title        NVARCHAR(100) NULL,
        CreatedAt    DATETIME     NOT NULL CONSTRAINT DF_ProductImages_CreatedAt DEFAULT(GETDATE()),
        UploadedAt   DATETIME     NOT NULL CONSTRAINT DF_ProductImages_UploadedAt DEFAULT(GETDATE()),
        IsActive     BIT          NOT NULL CONSTRAINT DF_ProductImages_IsActive DEFAULT(1),
        CreatedBy    NVARCHAR(50) NULL,
        CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProductImages_SysFiles FOREIGN KEY (SysFileId)
            REFERENCES dbo.SysFile(Id) ON DELETE SET NULL
    );

    CREATE INDEX IX_ProductImages_ProductId          ON dbo.ProductImages(ProductId);
    CREATE INDEX IX_ProductImages_ProductId_IsMain   ON dbo.ProductImages(ProductId, IsMain);
END
GO

/* -------------------------------------------------------------
   9. ProductSpecifications
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ProductSpecifications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductSpecifications
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductSpecifications PRIMARY KEY,
        ProductId INT          NULL,
        CPU       NVARCHAR(100) NULL,
        RAM       NVARCHAR(50)  NULL,
        Storage   NVARCHAR(100) NULL,
        GPU       NVARCHAR(100) NULL,
        Screen    NVARCHAR(100) NULL,
        OS        NVARCHAR(50)  NULL,
        Ports     NVARCHAR(255) NULL,
        Weight    NVARCHAR(50)  NULL,
        Battery   NVARCHAR(50)  NULL,
        CONSTRAINT FK_ProductSpecifications_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );
END
GO

/* -------------------------------------------------------------
   10. ProductReviews
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ProductReviews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductReviews
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductReviews PRIMARY KEY,
        ProductId INT           NULL,
        UserId    INT           NULL,
        Rating    INT           NULL,
        Comment   NVARCHAR(MAX) NULL,
        CreatedAt DATETIME      NULL CONSTRAINT DF_ProductReviews_CreatedAt DEFAULT(GETDATE()),
        CONSTRAINT FK_ProductReviews_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
        CONSTRAINT FK_ProductReviews_Users    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id),
        CONSTRAINT CK_ProductReviews_Rating CHECK (Rating IS NULL OR Rating BETWEEN 1 AND 5)
    );
END
GO

/* -------------------------------------------------------------
   11. ShoppingCarts + ShoppingCartItems
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ShoppingCarts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ShoppingCarts
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ShoppingCarts PRIMARY KEY,
        UserId      INT          NOT NULL,
        CreatedAt   DATETIME     NOT NULL CONSTRAINT DF_ShoppingCarts_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt   DATETIME     NOT NULL CONSTRAINT DF_ShoppingCarts_UpdatedAt DEFAULT(GETDATE()),
        TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_ShoppingCarts_Total DEFAULT(0),
        CONSTRAINT FK_ShoppingCarts_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'dbo.ShoppingCartItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ShoppingCartItems
    (
        Id             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ShoppingCartItems PRIMARY KEY,
        ShoppingCartId INT          NOT NULL,
        ProductId      INT          NOT NULL,
        Quantity       INT          NOT NULL CONSTRAINT DF_ShoppingCartItems_Quantity DEFAULT(1),
        AddedAt        DATETIME     NOT NULL CONSTRAINT DF_ShoppingCartItems_AddedAt DEFAULT(GETDATE()),
        UnitPrice      DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_ShoppingCartItems_ShoppingCarts FOREIGN KEY (ShoppingCartId)
            REFERENCES dbo.ShoppingCarts(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ShoppingCartItems_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(Id)
    );
END
GO

/* -------------------------------------------------------------
   12. Orders + OrderItems + OrderHistories + PaymentTransactions
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
        OrderNumber       NVARCHAR(50)  NOT NULL,
        UserId            INT           NULL,
        ShippingAddressId INT           NULL,
        OrderDate         DATETIME      NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT(GETDATE()),
        Status            NVARCHAR(50)  NOT NULL CONSTRAINT DF_Orders_Status DEFAULT(N'Pending'),
        SubTotal          DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_SubTotal DEFAULT(0),
        DiscountAmount    DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_DiscountAmount DEFAULT(0),
        DiscountCode      NVARCHAR(100) NULL,
        TaxAmount         DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_TaxAmount DEFAULT(0),
        ShippingFee       DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_ShippingFee DEFAULT(0),
        TotalAmount       DECIMAL(18,2) NOT NULL,
        ShippingMethod    NVARCHAR(50)  NULL,
        PaymentMethod     NVARCHAR(50)  NULL,
        IsPaid            BIT           NOT NULL CONSTRAINT DF_Orders_IsPaid DEFAULT(0),
        PaidDate          DATETIME      NULL,
        Notes             NVARCHAR(1000) NULL,
        CreatedAt         DATETIME      NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt         DATETIME      NULL,
        CreatedBy         NVARCHAR(100) NULL,
        UpdatedBy         NVARCHAR(100) NULL,
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Orders_UserAddresses FOREIGN KEY (ShippingAddressId) REFERENCES dbo.UserAddresses(Id)
    );

    CREATE UNIQUE INDEX UX_Orders_OrderNumber ON dbo.Orders(OrderNumber);
    CREATE INDEX IX_Orders_UserId   ON dbo.Orders(UserId);
    CREATE INDEX IX_Orders_Status   ON dbo.Orders(Status);
    CREATE INDEX IX_Orders_OrderDate ON dbo.Orders(OrderDate);
END
GO

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY,
        OrderId         INT           NOT NULL,
        ProductId       INT           NOT NULL,
        Quantity        INT           NOT NULL,
        UnitPrice       DECIMAL(18,2) NOT NULL,
        CostPrice       DECIMAL(18,2) NOT NULL CONSTRAINT DF_OrderItems_CostPrice DEFAULT(0),
        DiscountAmount  DECIMAL(18,2) NOT NULL CONSTRAINT DF_OrderItems_DiscountAmount DEFAULT(0),
        DiscountPercent DECIMAL(5,2)  NOT NULL CONSTRAINT DF_OrderItems_DiscountPercent DEFAULT(0),
        TaxAmount       DECIMAL(18,2) NOT NULL CONSTRAINT DF_OrderItems_TaxAmount DEFAULT(0),
        SubTotal        DECIMAL(18,2) NOT NULL,
        SKU             NVARCHAR(100) NULL,
        SerialNumber    NVARCHAR(100) NULL,
        Status          NVARCHAR(50)  NOT NULL CONSTRAINT DF_OrderItems_Status DEFAULT(N'Pending'),
        Notes           NVARCHAR(500) NULL,
        CONSTRAINT FK_OrderItems_Orders   FOREIGN KEY (OrderId)   REFERENCES dbo.Orders(Id)   ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );

    CREATE INDEX IX_OrderItems_OrderId   ON dbo.OrderItems(OrderId);
    CREATE INDEX IX_OrderItems_ProductId ON dbo.OrderItems(ProductId);
END
GO

IF OBJECT_ID(N'dbo.OrderHistories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderHistories
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderHistories PRIMARY KEY,
        OrderId   INT          NOT NULL,
        OldStatus NVARCHAR(50) NOT NULL,
        NewStatus NVARCHAR(50) NOT NULL,
        ChangedAt DATETIME     NOT NULL CONSTRAINT DF_OrderHistories_ChangedAt DEFAULT(GETDATE()),
        ChangedBy NVARCHAR(100) NULL,
        Notes     NVARCHAR(500) NULL,
        CONSTRAINT FK_OrderHistories_Orders FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders(Id) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'dbo.PaymentTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaymentTransactions
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PaymentTransactions PRIMARY KEY,
        OrderId         INT          NULL,
        PaymentMethod   NVARCHAR(50) NULL,
        Status          NVARCHAR(50) NULL,
        TransactionDate DATETIME     NULL CONSTRAINT DF_PaymentTransactions_Date DEFAULT(GETDATE()),
        CONSTRAINT FK_PaymentTransactions_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
    );
END
GO

/* -------------------------------------------------------------
   13. Inventories + InventoryHistories
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Inventories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Inventories
    (
        Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Inventories PRIMARY KEY,
        ProductId         INT          NOT NULL,
        CurrentStock      INT          NOT NULL CONSTRAINT DF_Inventories_CurrentStock DEFAULT(0),
        MinimumStock      INT          NOT NULL CONSTRAINT DF_Inventories_MinimumStock DEFAULT(5),
        ReorderPoint      INT          NOT NULL CONSTRAINT DF_Inventories_ReorderPoint DEFAULT(10),
        AverageCost       DECIMAL(18,2) NOT NULL CONSTRAINT DF_Inventories_AverageCost DEFAULT(0),
        LastPurchasePrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Inventories_LastPurchasePrice DEFAULT(0),
        LastUpdated       DATETIME      NOT NULL CONSTRAINT DF_Inventories_LastUpdated DEFAULT(GETDATE()),
        Location          NVARCHAR(100) NULL,
        Status            INT           NOT NULL CONSTRAINT DF_Inventories_Status DEFAULT(0), -- enum InventoryStatus
        CONSTRAINT FK_Inventories_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Inventories_ProductId ON dbo.Inventories(ProductId);
END
GO

IF OBJECT_ID(N'dbo.InventoryHistories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryHistories
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventoryHistories PRIMARY KEY,
        InventoryId     INT          NOT NULL,
        TransactionType NVARCHAR(50) NOT NULL,
        Quantity        INT          NOT NULL,
        UnitCost        DECIMAL(18,2) NOT NULL CONSTRAINT DF_InventoryHistories_UnitCost DEFAULT(0),
        TransactionDate DATETIME      NOT NULL CONSTRAINT DF_InventoryHistories_Date DEFAULT(GETDATE()),
        ReferenceId     INT          NULL,
        ReferenceType   NVARCHAR(50) NULL,
        Notes           NVARCHAR(500) NULL,
        CreatedBy       NVARCHAR(100) NULL,
        CONSTRAINT FK_InventoryHistories_Inventories FOREIGN KEY (InventoryId)
            REFERENCES dbo.Inventories(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_InventoryHistories_InventoryId ON dbo.InventoryHistories(InventoryId);
END
GO

/* -------------------------------------------------------------
   14. Suppliers + SupplierOrders + SupplierOrderItems
   ------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Suppliers
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Suppliers PRIMARY KEY,
        Name         NVARCHAR(150) NOT NULL,
        ContactName  NVARCHAR(100) NULL,
        Email        NVARCHAR(100) NULL,
        Phone        NVARCHAR(20)  NULL,
        Address      NVARCHAR(255) NULL,
        Website      NVARCHAR(255) NULL,
        Notes        NVARCHAR(1000) NULL,
        IsActive     BIT           NOT NULL CONSTRAINT DF_Suppliers_IsActive DEFAULT(1),
        CreatedAt    DATETIME      NOT NULL CONSTRAINT DF_Suppliers_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt    DATETIME      NULL
    );
END
GO

IF OBJECT_ID(N'dbo.SupplierOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupplierOrders
    (
        Id             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupplierOrders PRIMARY KEY,
        PurchaseNumber NVARCHAR(50)  NOT NULL,
        SupplierId     INT           NOT NULL,
        OrderDate      DATETIME      NOT NULL CONSTRAINT DF_SupplierOrders_OrderDate DEFAULT(GETDATE()),
        Status         NVARCHAR(50)  NOT NULL CONSTRAINT DF_SupplierOrders_Status DEFAULT(N'Pending'),
        TotalAmount    DECIMAL(18,2) NOT NULL CONSTRAINT DF_SupplierOrders_TotalAmount DEFAULT(0),
        DeliveryDate   DATETIME      NULL,
        Notes          NVARCHAR(500) NULL,
        CreatedAt      DATETIME      NOT NULL CONSTRAINT DF_SupplierOrders_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt      DATETIME      NULL,
        CreatedBy      NVARCHAR(100) NULL,
        CONSTRAINT FK_SupplierOrders_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
    );

    CREATE UNIQUE INDEX UX_SupplierOrders_PurchaseNumber ON dbo.SupplierOrders(PurchaseNumber);
END
GO

IF OBJECT_ID(N'dbo.SupplierOrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupplierOrderItems
    (
        Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupplierOrderItems PRIMARY KEY,
        SupplierOrderId  INT           NOT NULL,
        ProductId        INT           NOT NULL,
        Quantity         INT           NOT NULL,
        UnitCost         DECIMAL(18,2) NOT NULL,
        TotalCost        DECIMAL(18,2) NOT NULL,
        ReceivedQuantity INT           NOT NULL CONSTRAINT DF_SupplierOrderItems_Received DEFAULT(0),
        Notes            NVARCHAR(500) NULL,
        CONSTRAINT FK_SupplierOrderItems_SupplierOrders FOREIGN KEY (SupplierOrderId)
            REFERENCES dbo.SupplierOrders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SupplierOrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );
END
GO

PRINT N'>>> Done: current schema deployed (21 tables).';
GO

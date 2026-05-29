-- ============================================================
-- Wallet feature — manual migration script
-- Run on: Azure SQL (be-laptopshop.database.windows.net) hoặc local
-- Idempotent: kiểm tra IF NOT EXISTS trước khi tạo.
-- ============================================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

-- ===== 1. UserWallets =====
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserWallets')
BEGIN
    CREATE TABLE dbo.UserWallets
    (
        Id              int IDENTITY(1,1) NOT NULL,
        UserId          int               NOT NULL,
        Balance         decimal(18, 2)    NOT NULL CONSTRAINT DF_UserWallets_Balance       DEFAULT (0),
        LifetimeTopUp   decimal(18, 2)    NOT NULL CONSTRAINT DF_UserWallets_LifetimeTopUp DEFAULT (0),
        LifetimeSpent   decimal(18, 2)    NOT NULL CONSTRAINT DF_UserWallets_LifetimeSpent DEFAULT (0),
        IsActive        bit               NOT NULL CONSTRAINT DF_UserWallets_IsActive      DEFAULT (1),
        IsLocked        bit               NOT NULL CONSTRAINT DF_UserWallets_IsLocked      DEFAULT (0),
        LockReason      nvarchar(255)     NULL,
        CreatedAt       datetime2(7)      NOT NULL CONSTRAINT DF_UserWallets_CreatedAt     DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       datetime2(7)      NULL,
        RowVersion      rowversion        NOT NULL,
        CONSTRAINT PK_UserWallets PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_UserWallets_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_UserWallets_UserId ON dbo.UserWallets (UserId);

    PRINT 'Created table UserWallets.';
END
ELSE
    PRINT 'Table UserWallets already exists. Skipped.';

-- ===== 2. WalletTransactions (ledger) =====
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WalletTransactions')
BEGIN
    CREATE TABLE dbo.WalletTransactions
    (
        Id              bigint IDENTITY(1,1) NOT NULL,
        WalletId        int                  NOT NULL,
        UserId          int                  NOT NULL,
        [Type]          int                  NOT NULL,    -- enum WalletTransactionType
        Amount          decimal(18, 2)       NOT NULL,
        BalanceBefore   decimal(18, 2)       NOT NULL,
        BalanceAfter    decimal(18, 2)       NOT NULL,
        ReferenceType   nvarchar(50)         NULL,        -- "Order", "GiftCard", "Manual", ...
        ReferenceId     nvarchar(50)         NULL,
        Note            nvarchar(500)        NULL,
        CreatedBy       nvarchar(50)         NULL,
        CreatedAt       datetime2(7)         NOT NULL CONSTRAINT DF_WalletTransactions_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_WalletTransactions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_WalletTransactions_UserWallets FOREIGN KEY (WalletId)
            REFERENCES dbo.UserWallets (Id) ON DELETE CASCADE,
        CONSTRAINT FK_WalletTransactions_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_WalletTransactions_WalletId  ON dbo.WalletTransactions (WalletId);
    CREATE INDEX IX_WalletTransactions_UserId    ON dbo.WalletTransactions (UserId);
    CREATE INDEX IX_WalletTransactions_Reference ON dbo.WalletTransactions (ReferenceType, ReferenceId);
    CREATE INDEX IX_WalletTransactions_CreatedAt ON dbo.WalletTransactions (CreatedAt DESC);

    PRINT 'Created table WalletTransactions.';
END
ELSE
    PRINT 'Table WalletTransactions already exists. Skipped.';

COMMIT TRANSACTION;
PRINT '=== Wallet migration completed ===';

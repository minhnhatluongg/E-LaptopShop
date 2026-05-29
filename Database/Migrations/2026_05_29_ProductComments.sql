-- =============================================
-- Migration: Tạo bảng ProductComments cho trang chi tiết sản phẩm.
-- Tương thích với EF Core configuration: ProductCommentConfiguration.cs
-- Database: SQL Server (DefaultConnection)
-- Chạy 1 lần. An toàn idempotent (kiểm tra tồn tại trước khi tạo).
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ProductComments')
BEGIN
    CREATE TABLE [dbo].[ProductComments]
    (
        [Id]              INT            IDENTITY(1,1) NOT NULL,
        [ProductId]       INT            NOT NULL,
        [UserId]          INT            NOT NULL,
        [Content]         NVARCHAR(2000) NOT NULL,
        [ParentCommentId] INT            NULL,
        [IsDeleted]       BIT            NOT NULL CONSTRAINT [DF_ProductComments_IsDeleted] DEFAULT (0),
        [CreatedAt]       DATETIME       NOT NULL CONSTRAINT [DF_ProductComments_CreatedAt] DEFAULT (GETDATE()),
        [UpdatedAt]       DATETIME       NULL,

        CONSTRAINT [PK_ProductComments] PRIMARY KEY CLUSTERED ([Id] ASC),

        CONSTRAINT [FK_ProductComments_Products_ProductId]
            FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE,

        CONSTRAINT [FK_ProductComments_Users_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION,

        CONSTRAINT [FK_ProductComments_ProductComments_ParentCommentId]
            FOREIGN KEY ([ParentCommentId]) REFERENCES [dbo].[ProductComments] ([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_ProductComments_ProductId] ON [dbo].[ProductComments] ([ProductId]);
    CREATE INDEX [IX_ProductComments_UserId]    ON [dbo].[ProductComments] ([UserId]);

    PRINT 'Đã tạo bảng ProductComments.';
END
ELSE
BEGIN
    PRINT 'Bảng ProductComments đã tồn tại — bỏ qua.';
END
GO

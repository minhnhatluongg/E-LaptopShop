-- =============================================
-- Migration: Thêm cột UpdatedAt vào bảng Products.
-- Tương thích EF Core: ProductConfiguration.cs đã map UpdatedAt là DATETIME nullable.
-- Database: SQL Server (DefaultConnection)
-- Idempotent: kiểm tra COL_LENGTH trước khi ALTER.
--
-- Lưu ý: SQL Server không hỗ trợ "ALTER ... ADD <col> AFTER <other_col>".
-- Cột mới sẽ được thêm vào cuối bảng — vị trí không ảnh hưởng tới EF/code.
-- Nếu muốn UpdatedAt nằm ngay sau CreatedAt về mặt vật lý, phải rebuild
-- table (CREATE NEW + INSERT + DROP + RENAME) — không khuyến nghị cho prod.
-- =============================================

IF COL_LENGTH('dbo.Products', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE [dbo].[Products]
        ADD [UpdatedAt] DATETIME NULL;
    PRINT 'Đã thêm cột UpdatedAt vào bảng Products.';
END
ELSE
BEGIN
    PRINT 'Cột UpdatedAt đã tồn tại — bỏ qua.';
END
GO

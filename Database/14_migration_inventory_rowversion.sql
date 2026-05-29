/* ==============================================================
   Migration: Thêm RowVersion vào Inventories
   Dùng cho Optimistic Locking — ngăn Race Condition khi trừ kho.
   Idempotent: chỉ chạy nếu cột chưa tồn tại.
   ============================================================== */

SET NOCOUNT ON;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Inventories')
      AND name = N'RowVersion'
)
BEGIN
    ALTER TABLE dbo.Inventories
        ADD [RowVersion] rowversion NOT NULL;

    PRINT 'Added RowVersion column to Inventories.';
END
ELSE
    PRINT 'RowVersion column already exists. Skipped.';

-- Verify
SELECT TOP 3 Id, ProductId, CurrentStock, RowVersion
FROM dbo.Inventories
ORDER BY Id;

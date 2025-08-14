-- Script cập nhật roles cho E-LaptopShop 2025
-- Chỉ sử dụng các cột có sẵn: Id, Name, IsActive

-- 1. Update roles hiện tại (nếu tồn tại)
UPDATE Roles 
SET Name = 'Customer'
WHERE Name = 'User';

UPDATE Roles 
SET Name = 'Sales'
WHERE Name = 'Staff';

-- Admin giữ nguyên

-- 2. Thêm các roles mới cho 2025 (chỉ thêm nếu chưa tồn tại)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Customer')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Customer', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Admin', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Sales')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Sales', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Manager')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Manager', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Warehouse')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Warehouse', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Support')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Support', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Moderator')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Moderator', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'VIP')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('VIP', 1)
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Partner')
BEGIN
    INSERT INTO Roles (Name, IsActive)
    VALUES ('Partner', 1)
END

-- 3. Kiểm tra kết quả
SELECT Id, Name, IsActive 
FROM Roles 
ORDER BY Name;

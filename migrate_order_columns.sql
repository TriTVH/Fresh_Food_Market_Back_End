-- Script thêm cột mới vào bảng Order và OrderItem
-- Chạy trên database: OrderSer_Redis (Azure SQL Server)

-- 1. Thêm cột user_id vào bảng Order
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Order' AND COLUMN_NAME = 'user_id'
)
BEGIN
    ALTER TABLE [Order] ADD user_id NVARCHAR(100) NULL;
    PRINT 'Added user_id to [Order]';
END
ELSE
    PRINT 'user_id already exists in [Order]';

-- 2. Thêm cột product_name vào bảng OrderItem
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'OrderItem' AND COLUMN_NAME = 'product_name'
)
BEGIN
    ALTER TABLE [OrderItem] ADD product_name NVARCHAR(255) NULL;
    PRINT 'Added product_name to [OrderItem]';
END
ELSE
    PRINT 'product_name already exists in [OrderItem]';

-- 3. Thêm cột price vào bảng OrderItem
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'OrderItem' AND COLUMN_NAME = 'price'
)
BEGIN
    ALTER TABLE [OrderItem] ADD price DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT 'Added price to [OrderItem]';
END
ELSE
    PRINT 'price already exists in [OrderItem]';

-- 4. Thêm cột sub_total vào bảng OrderItem  
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'OrderItem' AND COLUMN_NAME = 'sub_total'
)
BEGIN
    ALTER TABLE [OrderItem] ADD sub_total DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT 'Added sub_total to [OrderItem]';
END
ELSE
    PRINT 'sub_total already exists in [OrderItem]';

PRINT 'Migration completed successfully!';

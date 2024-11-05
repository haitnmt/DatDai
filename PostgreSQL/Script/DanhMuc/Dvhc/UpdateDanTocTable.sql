-- Tạo Các Script Cập nhật cấu trúc bảng Dân tốc
-- Tạo bảng DanToc chỉ có Id, sau đó cập nhật cấu trúc bảng
DO $$
DECLARE
    table_name CONSTANT TEXT := 'DanToc';
    col_ten CONSTANT TEXT := 'TenDanToc';
    col_ghichu CONSTANT TEXT := 'TenGoiKhac';
    col_created CONSTANT TEXT := 'CreatedAt';
    col_updated CONSTANT TEXT := 'UpdatedAt';
BEGIN
    CREATE TABLE IF NOT EXISTS table_name (
        "Id" INT PRIMARY KEY
    );
END $$;

-- Cập nhật cấu trúc bảng DanToc

-- Kiểm tra tồn tại và cập nhật cột "Ten"
DO $$
DECLARE
    exists_column BOOLEAN;
BEGIN
    SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = table_name AND column_name = col_ten) INTO exists_column;
    IF NOT exists_column THEN
        EXECUTE format('ALTER TABLE %I ADD COLUMN %I VARCHAR(50)', table_name, col_ten);
    ELSE
        EXECUTE format('ALTER TABLE %I ALTER COLUMN %I TYPE VARCHAR(50)', table_name, col_ten);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "GhiChu"
DO $$
DECLARE
    exists_column BOOLEAN;
BEGIN
    SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = table_name AND column_name = col_ghichu) INTO exists_column;
    IF NOT exists_column THEN
        EXECUTE format('ALTER TABLE %I ADD COLUMN %I TEXT', table_name, col_ghichu);
    ELSE
        EXECUTE format('ALTER TABLE %I ALTER COLUMN %I TYPE TEXT', table_name, col_ghichu);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "CreatedAt"
DO $$
DECLARE
    exists_column BOOLEAN;
BEGIN
    SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = table_name AND column_name = col_created) INTO exists_column;
    IF NOT exists_column THEN
        EXECUTE format('ALTER TABLE %I ADD COLUMN %I TIMESTAMPTZ', table_name, col_created);
    ELSE
        EXECUTE format('ALTER TABLE %I ALTER COLUMN %I TYPE TIMESTAMPTZ', table_name, col_created);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "UpdatedAt"
DO $$
DECLARE
    exists_column BOOLEAN;
BEGIN
    SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = table_name AND column_name = col_updated) INTO exists_column;
    IF NOT exists_column THEN
        EXECUTE format('ALTER TABLE %I ADD COLUMN %I TIMESTAMPTZ', table_name, col_updated);
    ELSE
        EXECUTE format('ALTER TABLE %I ALTER COLUMN %I TYPE TIMESTAMPTZ', table_name, col_updated);
    END IF;
END $$;

-- Khởi tạo các Index
DO $$
BEGIN
    EXECUTE format('CREATE INDEX IF NOT EXISTS %I ON %I (%I)', 'IDX_DanToc_TenDanToc', table_name, col_ten);
    EXECUTE format('CREATE INDEX IF NOT EXISTS %I ON %I (%I)', 'IDX_DanToc_TenGoiKhac', table_name, col_ghichu);
END $$;
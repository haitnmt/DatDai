-- Tạo Các Script Cập nhật cấu trúc bảng Dân tốc
 -- Tạo bảng DanToc chỉ có Id, sau đó cập nhật cấu trúc bảng
DO $$
    BEGIN
        CREATE TABLE IF NOT EXISTS "DanToc" (
            "Id" INT PRIMARY KEY
        );
    END $$;

-- Cập nhật cấu trúc bảng DanToc

-- Kiểm tra tồn tại và cập nhật cột "Ten"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'TenDanToc')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "TenDanToc" VARCHAR(50);
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "TenDanToc" TYPE VARCHAR(50);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "GhiChu"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'TenGoiKhac')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "TenGoiKhac" TEXT;
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "TenGoiKhac" TYPE TEXT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "CreatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'CreatedAt')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "CreatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "CreatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "UpdatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'UpdatedAt')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "UpdatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "UpdatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Khởi tạo các Index
DO $$
BEGIN
    CREATE INDEX IF NOT EXISTS "IDX_DanToc_TenDanToc" ON "DanToc" ("TenDanToc");
    CREATE INDEX IF NOT EXISTS "IDX_DanToc_TenGoiKhac" ON "DanToc" ("TenGoiKhac");
END $$;
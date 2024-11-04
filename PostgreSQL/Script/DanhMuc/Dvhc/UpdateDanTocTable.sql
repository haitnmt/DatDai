-- Tạo Các Script Cập nhật cấu trúc bảng Dân tốc
 -- Tạo bảng DanToc chỉ có Id, sau đó cập nhật cấu trúc bảng
DO $$
    BEGIN
        CREATE TABLE IF NOT EXISTS "DanToc" (
            "Id" UUID PRIMARY KEY
        );
    END $$;

-- Cập nhật cấu trúc bảng DanToc
 -- Kiểm tra tồn tại và cập nhật cột "Ma"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'MaKyHieu')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "MaKyHieu" VARCHAR(2);
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "MaKyHieu" TYPE VARCHAR(2);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "Ten"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'TenGiaTri')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "TenGiaTri" VARCHAR(50);
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "TenGiaTri" TYPE VARCHAR(50);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "TenGoiKhac"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'TenGoiKhac')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "TenGoiKhac" VARCHAR(50)[];
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "TenGoiKhac" TYPE VARCHAR(50)[] USING "TenGoiKhac"::VARCHAR(50)[];
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "GhiChu"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'DanToc' AND column_name = 'GhiChu')
       THEN
            ALTER TABLE "DanToc" ADD COLUMN "GhiChu" TEXT;
        ELSE
            ALTER TABLE "DanToc" ALTER COLUMN "GhiChu" TYPE TEXT;
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
    CREATE INDEX IF NOT EXISTS "IDX_DanToc_MaKyHieu" ON "DanToc" ("MaKyHieu");
    CREATE INDEX IF NOT EXISTS "IDX_DanToc_TenGiaTri" ON "DanToc" ("TenGiaTri");
    CREATE INDEX IF NOT EXISTS "IDX_DanToc_TenGoiKhac" ON "DanToc" ("TenGoiKhac");
END $$;
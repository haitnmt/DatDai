-- Tạo Các Script Cập nhật cấu trúc bảng Dân tốc
 -- Tạo bảng QuocTich chỉ có Id, sau đó cập nhật cấu trúc bảng
DO $$
    BEGIN
        CREATE TABLE IF NOT EXISTS "QuocTich" (
            "Id" uuid PRIMARY KEY
        );
    END $$;

-- Cập nhật cấu trúc bảng QuocTich

-- Kiểm tra tồn tại và cập nhật cột "ccn3"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'ccn3')
       THEN
            ALTER TABLE "QuocTich" ADD COLUMN "ccn3" INT;
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "ccn3" TYPE INT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "cca3"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'cca3')
        THEN
            ALTER TABLE "QuocTich" ADD COLUMN "cca3" VARCHAR(3);
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "cca3" TYPE VARCHAR(3);
        END IF;
    END $$;


-- Kiểm tra tồn tại và cập nhật cột "TenQuocGia"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'TenQuocGia')
        THEN
            ALTER TABLE "QuocTich" ADD COLUMN "TenQuocGia" VARCHAR(50);
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "TenQuocGia" TYPE VARCHAR(50);
        END IF;
    END $$;


-- Kiểm tra tồn tại và cập nhật cột "TenDayDu"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'TenDayDu')
        THEN
            ALTER TABLE "QuocTich" ADD COLUMN "TenDayDu" VARCHAR(150);
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "TenDayDu" TYPE VARCHAR(150);
        END IF;
    END $$;


-- Kiểm tra tồn tại và cập nhật cột "TenQuocTe"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'TenQuocTe')
        THEN
            ALTER TABLE "QuocTich" ADD COLUMN "TenQuocTe" VARCHAR(50);
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "TenQuocTe" TYPE VARCHAR(50);
        END IF;
    END $$;


-- Kiểm tra tồn tại và cập nhật cột "TenQuocTeDayDu"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'TenQuocTeDayDu')
        THEN
            ALTER TABLE "QuocTich" ADD COLUMN "TenQuocTeDayDu" VARCHAR(150);
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "TenQuocTeDayDu" TYPE VARCHAR(150);
        END IF;
    END $$;



-- Kiểm tra tồn tại và cập nhật cột "GhiChu"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'GhiChu')
       THEN
            ALTER TABLE "QuocTich" ADD COLUMN "GhiChu" TEXT;
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "GhiChu" TYPE TEXT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "CreatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'CreatedAt')
       THEN
            ALTER TABLE "QuocTich" ADD COLUMN "CreatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "CreatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "UpdatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'QuocTich' AND column_name = 'UpdatedAt')
       THEN
            ALTER TABLE "QuocTich" ADD COLUMN "UpdatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "QuocTich" ALTER COLUMN "UpdatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Khởi tạo các Index
DO $$
BEGIN
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_ccn3" ON "QuocTich" ("ccn3");
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_cca3" ON "QuocTich" ("cca3");
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocGia" ON "QuocTich" ("TenQuocGia");
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenDayDu" ON "QuocTich" ("TenDayDu");
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocTe" ON "QuocTich" ("TenQuocTe");
    CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocTeDayDu" ON "QuocTich" ("TenQuocTeDayDu");
END $$;
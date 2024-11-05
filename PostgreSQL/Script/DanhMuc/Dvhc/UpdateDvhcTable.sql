-- Tạo Các Script Cập nhật cấu trúc bảng Dvhc   
    
-- Kiểm tra bảng Dvhc có tồn tại không, nếu chưa tồn tại thì tạo bảng Dvhc chỉ có Id, sau đó cập nhât cấu trúc bảng
DO $$
    BEGIN
        CREATE TABLE IF NOT EXISTS "Dvhc" (
            "Id" uuid PRIMARY KEY
        );
    END $$;

-- Cập nhật cấu trúc bảng Dvhc

-- Kiểm tra tồn tại và cập nhật cột "MaXa"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'MaXa') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "MaXa" INT;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "MaXa" TYPE INT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "MaHuyen"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'MaHuyen') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "MaHuyen" INT;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "MaHuyen" TYPE INT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "MaTinh"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'MaTinh') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "MaTinh" INT;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "MaTinh" TYPE INT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "TenGiaTri"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'TenDvhc') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "TenDvhc" VARCHAR(150);
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "TenDvhc" TYPE VARCHAR(150);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "Cap"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'Cap') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "Cap" INT;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "Cap" TYPE INT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "LoaiHinh"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'LoaiHinh') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "LoaiHinh" VARCHAR(50);
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "LoaiHinh" TYPE VARCHAR(50);
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "NgayHieuLuc"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'NgayHieuLuc') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "NgayHieuLuc" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "NgayHieuLuc" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "HieuLuc"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'HieuLuc') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "HieuLuc" BOOLEAN;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "HieuLuc" TYPE BOOLEAN;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "GhiChu"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'GhiChu') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "GhiChu" TEXT;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "GhiChu" TYPE TEXT;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "CreatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'CreatedAt') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "CreatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "CreatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Kiểm tra tồn tại và cập nhật cột "UpdatedAt"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Dvhc' AND column_name = 'UpdatedAt') 
       THEN
            ALTER TABLE "Dvhc" ADD COLUMN "UpdatedAt" TIMESTAMPTZ;
        ELSE
            ALTER TABLE "Dvhc" ALTER COLUMN "UpdatedAt" TYPE TIMESTAMPTZ;
    END IF;
END $$;

-- Tạo các Index cho bảng Dvhc
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'Dvhc' AND indexname = 'IDX_Dvhc_MaXa') 
       THEN
            CREATE INDEX "IDX_Dvhc_MaXa" ON "Dvhc" ("MaXa") WHERE "MaXa" IS NOT NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'Dvhc' AND indexname = 'IDX_Dvhc_MaHuyen') 
       THEN
            CREATE INDEX "IDX_Dvhc_MaHuyen" ON "Dvhc" ("MaHuyen") WHERE "MaHuyen" IS NOT NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'Dvhc' AND indexname = 'IDX_Dvhc_MaTinh') 
       THEN
            CREATE INDEX "IDX_Dvhc_MaTinh" ON "Dvhc" ("MaTinh");
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'Dvhc' AND indexname = 'IDX_Dvhc_Ten') 
       THEN
            CREATE INDEX "IDX_Dvhc_Ten" ON "Dvhc" ("TenDvhc");
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'Dvhc' AND indexname = 'IDX_Dvhc_HieuLuc') 
       THEN
            CREATE INDEX "IDX_Dvhc_HieuLuc" ON "Dvhc" ("HieuLuc") WHERE "HieuLuc" = TRUE;
    END IF;
END $$;


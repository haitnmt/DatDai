-- Date: 2021-07-29 10:00:00
-- Author: Haihv
-- Description: Create table QuocTich

CREATE TABLE "QuocTich" (
                          "Id" uuid PRIMARY KEY,
                          "ccn3" INT,
                          "cca3" VARCHAR(3),
                          "TenQuocGia" VARCHAR(50),
                          "TenDayDu" VARCHAR(150),
                          "TenQuocTe" VARCHAR(50),
                          "TenQuocTeDayDu" VARCHAR(150),
                          "GhiChu" TEXT,
                          "CreatedAt" TIMESTAMPTZ,
                          "UpdatedAt" TIMESTAMPTZ,
                          "IsDeleted" BOOLEAN DEFAULT FALSE,
                          "DeletedAtUtc" TIMESTAMPTZ
);

-- Description: Create index for table QuocTich

CREATE INDEX IF NOT EXISTS "IDX_QuocTich_ccn3" ON "QuocTich" ("ccn3");
CREATE INDEX IF NOT EXISTS "IDX_QuocTich_cca3" ON "QuocTich" ("cca3");
CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocGia" ON "QuocTich" ("TenQuocGia");
CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenDayDu" ON "QuocTich" ("TenDayDu");
CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocTe" ON "QuocTich" ("TenQuocTe");
CREATE INDEX IF NOT EXISTS "IDX_QuocTich_TenQuocTeDayDu" ON "QuocTich" ("TenQuocTeDayDu");
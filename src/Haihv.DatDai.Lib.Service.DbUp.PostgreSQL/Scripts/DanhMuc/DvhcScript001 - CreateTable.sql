-- Date: 2024-10-29 15:00:00
-- Author: Haihv
-- Description: Create table Dvhc

CREATE TABLE "Dvhc" (
                      "Id" uuid PRIMARY KEY,
                      "MaXa" INT,
                      "MaHuyen" INT,
                      "MaTinh" INT,
                      "TenDvhc" VARCHAR(150),
                      "Cap" INT,
                      "LoaiHinh" VARCHAR(50),
                      "NgayHieuLuc" TIMESTAMPTZ,
                      "HieuLuc" BOOLEAN,
                      "GhiChu" TEXT,
                      "CreatedAt" TIMESTAMPTZ,
                      "UpdatedAt" TIMESTAMPTZ,
                      "IsDeleted" BOOLEAN DEFAULT FALSE,
                      "DeletedAt" TIMESTAMPTZ
);


-- Description: Create index for table Dvhc

CREATE INDEX "IDX_Dvhc_MaXa" ON "Dvhc" ("MaXa") WHERE "MaXa" IS NOT NULL;
CREATE INDEX "IDX_Dvhc_MaHuyen" ON "Dvhc" ("MaHuyen") WHERE "MaHuyen" IS NOT NULL;
CREATE INDEX "IDX_Dvhc_MaTinh" ON "Dvhc" ("MaTinh") WHERE "MaTinh" IS NOT NULL;
CREATE INDEX "IDX_Dvhc_Ten" ON "Dvhc" ("TenDvhc");
CREATE INDEX "IDX_Dvhc_HieuLuc" ON "Dvhc" ("HieuLuc") WHERE "HieuLuc" = TRUE;
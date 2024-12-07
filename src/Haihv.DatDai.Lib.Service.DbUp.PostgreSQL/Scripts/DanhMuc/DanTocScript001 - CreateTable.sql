-- Date: 2024-10-29 10:00:00
-- Author: Haihv
-- Description: Create table DanToc

CREATE TABLE "DanToc" (
                        "Id" uuid PRIMARY KEY,
                        "MaDanToc" INT,
                        "TenDanToc" VARCHAR(50),
                        "TenGoiKhac" TEXT,
                        "CreatedAt" TIMESTAMPTZ,
                        "UpdatedAt" TIMESTAMPTZ,
                        "IsDeleted" BOOLEAN DEFAULT FALSE,
                        "DeletedAt" TIMESTAMPTZ
);

-- Description: Create index for table DanToc
CREATE INDEX "IDX_DanToc_TenDanToc" ON "DanToc" ("TenDanToc");
CREATE INDEX "IDX_DanToc_TenGoiKhac" ON "DanToc" ("TenGoiKhac");
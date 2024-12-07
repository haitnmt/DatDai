-- Date: 2024-12-02 14:00:00
-- Update: 2024-12-04 08:00:00
-- Author: Haihv
-- Description: Create table Group

CREATE TABLE "Groups" (
    "Id" uuid PRIMARY KEY,
    "GroupName" VARCHAR(50),
    "DistinguishedName" VARCHAR(150),
    "MemberOf" uuid[],
    "GroupType" INTEGER,
    "GhiChu" VARCHAR(250),
    "CreatedAt" TIMESTAMPTZ,
    "UpdatedAt" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAt" TIMESTAMPTZ
);

-- Description: Create index for table Groups

CREATE INDEX "IDX_Groups_GroupName" ON "Groups" ("GroupName");
CREATE INDEX "IDX_Groups_DistinguishedName" ON "Groups" ("DistinguishedName");
CREATE INDEX "IDX_Groups_UpdatedAt" ON "Groups" ("UpdatedAt");
CREATE INDEX "IDX_Groups_GroupType" ON "Groups" ("GroupType");
CREATE INDEX "IDX_Groups_IsDeleted" ON "Groups" ("IsDeleted");
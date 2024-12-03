-- Date: 2024-12-02 14:00:00
-- Author: Haihv
-- Description: Create table Group

CREATE TABLE "Groups" (
    "Id" uuid PRIMARY KEY,
    "GroupName" VARCHAR(50),
    "MemberOf" uuid[],
    "GhiChu" VARCHAR(250),
    "WhenCreated" TIMESTAMPTZ,
    "WhenChanged" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAtUtc" TIMESTAMPTZ
);

-- Description: Create index for table Groups

CREATE INDEX "IDX_Groups_GroupName" ON "Groups" ("GroupName");
CREATE INDEX "IDX_Groups_IsDeleted" ON "Groups" ("IsDeleted");
-- Date: 2024-12-02 14:00:00
-- Author: Haihv
-- Description: Create table Group

CREATE TABLE "Group" (
    "Id" uuid PRIMARY KEY,
    "GroupName" VARCHAR(50),
    "MemberOf" uuid[],
    "GhiChu" VARCHAR(250),
    "WhenCreated" TIMESTAMPTZ,
    "WhenChanged" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAtUtc" TIMESTAMPTZ
);

-- Description: Create index for table Group

CREATE INDEX "IDX_Group_GroupName" ON "Group" ("GroupName");
CREATE INDEX "IDX_Group_IsDeleted" ON "Group" ("IsDeleted");
-- Date: 2024-12-02 14:00:00
-- Author: Haihv
-- Description: Create table UserGroup

CREATE TABLE "UserGroup" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "GroupId" uuid NOT NULL,
    "GhiChu" VARCHAR(250),
    "CreatedAt" TIMESTAMPTZ,
    "UpdatedAt" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAtUtc" TIMESTAMPTZ,
    CONSTRAINT "FK_UserGroup_User" FOREIGN KEY ("UserId") REFERENCES "User" ("Id"),
    CONSTRAINT "FK_UserGroup_Group" FOREIGN KEY ("GroupId") REFERENCES "Group" ("Id")
);

-- Description: Create index for table UserGroup

CREATE INDEX "IDX_UserGroup_UserId" ON "UserGroup" ("UserId");
CREATE INDEX "IDX_UserGroup_GroupId" ON "UserGroup" ("GroupId");
CREATE INDEX "IDX_UserGroup_IsDeleted" ON "UserGroup" ("IsDeleted");
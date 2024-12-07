-- Date: 2024-12-02 14:00:00
-- Author: Haihv
-- Description: Create table UserGroups

CREATE TABLE "UserGroups" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "GroupId" uuid NOT NULL,
    "GhiChu" VARCHAR(250),
    "CreatedAt" TIMESTAMPTZ,
    "UpdatedAt" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAt" TIMESTAMPTZ,
    CONSTRAINT "FK_UserGroups_User" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id"),
    CONSTRAINT "FK_UserGroups_Group" FOREIGN KEY ("GroupId") REFERENCES "Groups" ("Id")
);

-- Description: Create index for table UserGroups

CREATE INDEX "IDX_UserGroups_UserId" ON "UserGroups" ("UserId");
CREATE INDEX "IDX_UserGroups_GroupId" ON "UserGroups" ("GroupId");
CREATE INDEX "IDX_UserGroups_IsDeleted" ON "UserGroups" ("IsDeleted");
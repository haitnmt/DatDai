-- Date: 2024-12-02 13:00:00
-- Author: Haihv
-- Description: Create table User

CREATE TABLE "User" (
    "Id" uuid PRIMARY KEY,
    "UserName" VARCHAR(50),
    "Email" VARCHAR(50),
    "DisplayName" VARCHAR(88),
    "JobTitle" VARCHAR(150),
    "Description" VARCHAR(250),
    "Department" VARCHAR(150),
    "Organization" VARCHAR(150),
    "DomainUrl" VARCHAR(150),
    "IsLocked" BOOLEAN,
    "IsPwdMustChange" BOOLEAN,
    "PwdLastSet" TIMESTAMPTZ,
    "HashPassword" VARCHAR(64),
    "AuthenticationType" INTEGER,
    "GhiChu" VARCHAR(250),
    "WhenCreated" TIMESTAMPTZ,
    "WhenChanged" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAtUtc" TIMESTAMPTZ
);
-- Description: Create index for table User

CREATE INDEX "IDX_User_UserName" ON "User" ("UserName");
CREATE INDEX "IDX_User_Email" ON "User" ("Email");
CREATE INDEX "IDX_User_IsLocked" ON "User" ("IsLocked");
CREATE INDEX "IDX_User_IsPwdMustChange" ON "User" ("IsPwdMustChange");
CREATE INDEX "IDX_User_AuthenticationType" ON "User" ("AuthenticationType");
CREATE INDEX "IDX_User_WhenChanged" ON "User" ("WhenChanged");
CREATE INDEX "IDX_User_IsDeleted" ON "User" ("IsDeleted");
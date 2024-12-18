-- Date: 2024-12-02 13:00:00
-- Update: 2024-12-04 08:00:00
-- Author: Haihv
-- Description: Create table User

CREATE TABLE "Users" (
    "Id" uuid PRIMARY KEY,
    "UserName" VARCHAR(50),
    "DistinguishedName" VARCHAR(150),
    "Email" VARCHAR(50),
    "DisplayName" VARCHAR(88),
    "JobTitle" VARCHAR(150),
    "Department" VARCHAR(150),
    "Organization" VARCHAR(150),
    "DomainUrl" VARCHAR(150),
    "IsLocked" BOOLEAN,
    "IsPwdMustChange" BOOLEAN,
    "PwdLastSet" TIMESTAMPTZ,
    "HashPassword" VARCHAR(64),
    "AuthenticationType" INTEGER,
    "GhiChu" VARCHAR(250),
    "CreatedAt" TIMESTAMPTZ,
    "UpdatedAt" TIMESTAMPTZ,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "DeletedAt" TIMESTAMPTZ
);

-- Description: Create index for table User
CREATE INDEX "IDX_Users_UserName" ON "Users" ("UserName");
CREATE INDEX "IDX_Users_Email" ON "Users" ("Email");
CREATE INDEX "IDX_Users_DistinguishedName" ON "Users" ("DistinguishedName");
CREATE INDEX "IDX_Users_IsLocked" ON "Users" ("IsLocked");
CREATE INDEX "IDX_Users_IsPwdMustChange" ON "Users" ("IsPwdMustChange");
CREATE INDEX "IDX_Users_AuthenticationType" ON "Users" ("AuthenticationType");
CREATE INDEX "IDX_Users_UpdatedAt" ON "Users" ("UpdatedAt");
CREATE INDEX "IDX_Users_IsDeleted" ON "Users" ("IsDeleted");
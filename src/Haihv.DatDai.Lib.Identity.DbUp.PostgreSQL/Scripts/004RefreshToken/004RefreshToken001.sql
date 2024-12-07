-- Date: 2024-12-07 13:00:00
-- Author: Haihv
-- Description: Create table RefreshTokens

CREATE TABLE "RefreshTokens" (
    "Id" uuid PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Token" text NOT NULL,
    "Expires" TIMESTAMPTZ NOT NULL,
    CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Description: Create index for table UserGroups

CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");
CREATE INDEX "IX_RefreshTokens_Expires" ON "RefreshTokens" ("Expires");
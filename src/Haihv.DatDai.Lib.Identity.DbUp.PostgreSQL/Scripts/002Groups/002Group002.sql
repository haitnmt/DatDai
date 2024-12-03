-- Date: 2024-12-02 14:00:00
-- Author: Haihv
-- Description: Add new columns and indexes to table Group

ALTER TABLE "Groups"
    ADD COLUMN "GroupType" INTEGER;

CREATE INDEX "IDX_Groups_GroupType" ON "Groups" ("GroupType");
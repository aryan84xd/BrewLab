-- BrewLab Database Schema
-- PostgreSQL Migration Script

-- Drop tables if they exist (in correct order to avoid FK violations)
DROP TABLE IF EXISTS "Experiments" CASCADE;
DROP TABLE IF EXISTS "Coffees" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;

-- Create Users table
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL
);

-- Create Coffees table
CREATE TABLE "Coffees" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Brand" VARCHAR(255) NOT NULL,
    "Roast" VARCHAR(100) NOT NULL,
    "Origin" VARCHAR(255),
    "TastingNotes" TEXT,
    "UserId" UUID NOT NULL,
    CONSTRAINT "FK_Coffees_Users" FOREIGN KEY ("UserId") 
        REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create Experiments table
CREATE TABLE "Experiments" (
    "Id" UUID PRIMARY KEY,
    "Date" TIMESTAMP NOT NULL,
    "BrewMethod" VARCHAR(255),
    "CoffeeWeight" DECIMAL(18,2) NOT NULL,
    "WaterWeight" DECIMAL(18,2) NOT NULL,
    "BrewTime" TIME NOT NULL,
    "Remark" TEXT,
    "Aroma" INTEGER NOT NULL,
    "Acidity" INTEGER NOT NULL,
    "Body" INTEGER NOT NULL,
    "Overall" INTEGER NOT NULL,
    "CoffeeId" UUID NOT NULL,
    "UserId" UUID NOT NULL,
    CONSTRAINT "FK_Experiments_Coffees" FOREIGN KEY ("CoffeeId") 
        REFERENCES "Coffees"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Experiments_Users" FOREIGN KEY ("UserId") 
        REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create indexes for better query performance
CREATE INDEX "IX_Coffees_UserId" ON "Coffees"("UserId");
CREATE INDEX "IX_Experiments_CoffeeId" ON "Experiments"("CoffeeId");
CREATE INDEX "IX_Experiments_UserId" ON "Experiments"("UserId");
CREATE INDEX "IX_Users_Email" ON "Users"("Email");

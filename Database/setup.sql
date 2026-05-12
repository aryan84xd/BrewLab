-- BrewLab Database Setup Script
-- Run this script in your PostgreSQL database after creating the 'brewlab' database

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(500) NOT NULL
);

-- Create Coffees table
CREATE TABLE IF NOT EXISTS "Coffees" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Brand" VARCHAR(255) NOT NULL,
    "Roast" VARCHAR(100) NOT NULL,
    "Origin" VARCHAR(255),
    "TastingNotes" TEXT,
    "UserId" UUID NOT NULL,
    CONSTRAINT "FK_Coffees_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create Experiments table
CREATE TABLE IF NOT EXISTS "Experiments" (
    "Id" UUID PRIMARY KEY,
    "Date" TIMESTAMP NOT NULL,
    "BrewMethod" VARCHAR(100),
    "CoffeeWeight" DECIMAL(10,2) NOT NULL,
    "WaterWeight" DECIMAL(10,2) NOT NULL,
    "BrewTime" TIME NOT NULL,
    "Remark" TEXT,
    "Aroma" INTEGER NOT NULL,
    "Acidity" INTEGER NOT NULL,
    "Body" INTEGER NOT NULL,
    "Overall" INTEGER NOT NULL,
    "CoffeeId" UUID NOT NULL,
    "UserId" UUID NOT NULL,
    CONSTRAINT "FK_Experiments_Coffees" FOREIGN KEY ("CoffeeId") REFERENCES "Coffees"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Experiments_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_Coffees_UserId" ON "Coffees"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Experiments_CoffeeId" ON "Experiments"("CoffeeId");
CREATE INDEX IF NOT EXISTS "IX_Experiments_UserId" ON "Experiments"("UserId");

-- Verify tables were created
SELECT 
    table_name,
    table_type
FROM 
    information_schema.tables
WHERE 
    table_schema = 'public'
    AND table_name IN ('Users', 'Coffees', 'Experiments')
ORDER BY 
    table_name;

PRINT 'Database setup completed successfully!';

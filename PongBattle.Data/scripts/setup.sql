-- Create the database if it doesn't already exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PongBattle')
BEGIN
    CREATE DATABASE PongBattle;
END;
GO

-- Switch to the new database context
USE PongBattle;
GO

-- Create the  table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EmailAddress NVARCHAR(100) NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(10) NULL,
    );
END;
GO

-- Clear existing data to make the script re-runnable for demos
TRUNCATE TABLE Users;
GO

-- Insert some fresh data
INSERT INTO Users (EmailAddress, FirstName, LastName, PhoneNumber) VALUES ('dsnoble@stackoverflow.com', 'David', 'Snoble', '7805126101');
INSERT INTO Users (EmailAddress, FirstName, LastName) VALUES ('example@example.com', 'Example', 'Admin');

GO

PRINT 'Database and table created successfully. Data inserted.';
GO

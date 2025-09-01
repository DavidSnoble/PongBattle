-- Create the database if it doesn't already exist
IF NOT EXISTS (SELECT name
               FROM sys.databases
               WHERE name = N'PongBattle')
    BEGIN
        CREATE DATABASE PongBattle;
    END;
GO

-- Switch to the new database context
USE PongBattle;
GO

-- Create the Users table if it doesn't exist
IF NOT EXISTS (SELECT *
               FROM sys.objects
               WHERE object_id = OBJECT_ID(N'[dbo].[Users]')
                 AND type in (N'U'))
    BEGIN
        CREATE TABLE Users
        (
            Id           INT PRIMARY KEY IDENTITY (1,1),
            -- Added a UNIQUE constraint to prevent duplicate emails
            EmailAddress NVARCHAR(100) NOT NULL UNIQUE,
            FirstName    NVARCHAR(100) NOT NULL,
            LastName     NVARCHAR(100) NOT NULL,
            PhoneNumber  NVARCHAR(10)  NULL
        );
    END;
GO

-- Create the Teams table if it doesn't exist
IF NOT EXISTS (SELECT *
               FROM sys.objects
               WHERE object_id = OBJECT_ID(N'[dbo].[Teams]')
                 AND type in (N'U'))
    BEGIN
        CREATE TABLE Teams
        (
            Id          INT PRIMARY KEY IDENTITY (1,1),
            Name        NVARCHAR(100) NOT NULL,
            PlayerOneId INT           NOT NULL FOREIGN KEY REFERENCES Users (Id),
            PlayerTwoId INT           NULL FOREIGN KEY REFERENCES Users (Id)
        );
    END;
GO

-- Clear existing data to make the script re-runnable for demos
-- FIX: Use DELETE instead of TRUNCATE to handle the FOREIGN KEY constraint.
-- You must delete from the 'child' table (Teams) before the 'parent' table (Users).
DELETE
FROM Teams;
DELETE
FROM Users;
GO

-- Insert some fresh data
INSERT INTO Users (EmailAddress, FirstName, LastName, PhoneNumber)
VALUES ('dsnoble@stackoverflow.com', 'David', 'Snoble', '7805126101');

INSERT INTO Users (EmailAddress, FirstName, LastName)
VALUES ('example@example.com', 'Example', 'Admin');

INSERT INTO Teams (Name, PlayerOneId)
VALUES ('Funk Hunters',
        (SELECT Id
         FROM Users
         WHERE EmailAddress = 'dsnoble@stackoverflow.com'));

INSERT INTO Teams (Name, PlayerOneId, PlayerTwoId)
VALUES ('Funk Hunters',
        (SELECT Id
         FROM Users
         WHERE EmailAddress = 'dsnoble@stackoverflow.com'),
        (SELECT Id FROM Users WHERE EmailAddress = 'example@example.com'));
GO

PRINT 'Database and table created successfully. Data inserted.';
GO
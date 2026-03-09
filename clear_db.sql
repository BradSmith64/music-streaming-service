USE [music-streaming-db];
GO

-- Disable foreign key constraints
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"

-- Delete data in order to minimize friction (though constraints are disabled)
DELETE FROM [Likes];
DELETE FROM [Songs];
DELETE FROM [Albums];
DELETE FROM [Artists];

-- Reset identity columns for a truly fresh start
DBCC CHECKIDENT ('Likes', RESEED, 0);
DBCC CHECKIDENT ('Songs', RESEED, 0);
DBCC CHECKIDENT ('Albums', RESEED, 0);
DBCC CHECKIDENT ('Artists', RESEED, 0);

-- Re-enable foreign key constraints
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"

-- Verify
SELECT 'Artists' as TableName, COUNT(*) as [Count] FROM [Artists]
UNION ALL
SELECT 'Albums', COUNT(*) FROM [Albums]
UNION ALL
SELECT 'Songs', COUNT(*) FROM [Songs]
UNION ALL
SELECT 'Likes', COUNT(*) FROM [Likes];
GO

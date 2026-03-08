-- Set IDENTITY_INSERT OFF by default
-- Clear existing data
DELETE FROM [Likes];
DELETE FROM [Songs];
DELETE FROM [Albums];
DELETE FROM [Artists];

-- Insert Artists
SET IDENTITY_INSERT [Artists] ON;
INSERT INTO [Artists] (Id, Name) VALUES (1, 'Queen');
INSERT INTO [Artists] (Id, Name) VALUES (2, 'The Beatles');
INSERT INTO [Artists] (Id, Name) VALUES (3, 'David Bowie');
SET IDENTITY_INSERT [Artists] OFF;

-- Insert Albums
SET IDENTITY_INSERT [Albums] ON;
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (1, 'A Night at the Opera', 1);
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (2, 'Abbey Road', 2);
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (3, 'The Rise and Fall of Ziggy Stardust', 3);
SET IDENTITY_INSERT [Albums] OFF;

-- Insert Songs
SET IDENTITY_INSERT [Songs] ON;
INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (1, 'Bohemian Rhapsody', 1, 'seed-user-a-night-at-the-opera-bohemian-rhapsody.mp3', '1975-10-31');

INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (2, 'Come Together', 2, 'seed-user-abbey-road-come-together.mp3', '1969-09-26');

INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (3, 'Starman', 3, 'seed-user-the-rise-and-fall-of-ziggy-stardust-starman.mp3', '1972-04-28');
SET IDENTITY_INSERT [Songs] OFF;

-- Insert Likes
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (1, 101, GETUTCDATE());
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (1, 102, GETUTCDATE());
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (2, 101, GETUTCDATE());
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (3, 103, GETUTCDATE());
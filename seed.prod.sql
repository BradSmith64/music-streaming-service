-- PRODUCTION ANONYMIZED SEED DATA
-- Clear existing data
DELETE FROM [Likes];
DELETE FROM [Songs];
DELETE FROM [Albums];
DELETE FROM [Artists];

-- Insert Anonymized Artists
SET IDENTITY_INSERT [Artists] ON;
INSERT INTO [Artists] (Id, Name) VALUES (1, 'Artist Alpha');
INSERT INTO [Artists] (Id, Name) VALUES (2, 'Artist Beta');
INSERT INTO [Artists] (Id, Name) VALUES (3, 'Artist Gamma');
SET IDENTITY_INSERT [Artists] OFF;

-- Insert Anonymized Albums
SET IDENTITY_INSERT [Albums] ON;
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (1, 'Album One', 1);
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (2, 'Album Two', 2);
INSERT INTO [Albums] (Id, Title, ArtistId) VALUES (3, 'Album Three', 3);
SET IDENTITY_INSERT [Albums] OFF;

-- Insert Anonymized Songs
SET IDENTITY_INSERT [Songs] ON;
INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (1, 'Song A', 1, 'seed-user-album-one-song-a.mp3', '2020-01-01');

INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (2, 'Song B', 2, 'seed-user-album-two-song-b.mp3', '2021-06-15');

INSERT INTO [Songs] (Id, Title, AlbumId, FileName, ReleaseDate) 
VALUES (3, 'Song C', 3, 'seed-user-album-three-song-c.mp3', '2022-12-31');
SET IDENTITY_INSERT [Songs] OFF;

-- Insert Minimal Sample Likes
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (1, 1001, GETUTCDATE());
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (2, 1001, GETUTCDATE());
INSERT INTO [Likes] (SongId, UserId, CreatedAt) VALUES (3, 1002, GETUTCDATE());
USE [music-streaming-db];
GO

-- Enable identity insert to allow explicit IDs if needed, 
-- or just let the DB handle it if you don't care about matching the exact IDs from the mock.
-- Since the mock uses 1-20, let's try to match them.

SET IDENTITY_INSERT [Songs] ON;

INSERT INTO [Songs] ([Id], [Title], [AlbumTitle], [ReleaseDate], [FileName]) VALUES
(1, 'Song 1', 'Album 1', GETUTCDATE(), 'sample1.mp3'),
(2, 'Song 2', 'Album 2', GETUTCDATE(), 'sample2.wav'),
(3, 'Song 3', 'Album 3', GETUTCDATE(), 'sample3.wav'),
(4, 'Song 4', 'Album 4', GETUTCDATE(), 'sample1.mp3'),
(5, 'Song 5', 'Album 5', GETUTCDATE(), 'sample2.wav'),
(6, 'Song 6', 'Album 6', GETUTCDATE(), 'sample3.wav'),
(7, 'Song 7', 'Album 7', GETUTCDATE(), 'sample1.mp3'),
(8, 'Song 8', 'Album 8', GETUTCDATE(), 'sample2.wav'),
(9, 'Song 9', 'Album 9', GETUTCDATE(), 'sample3.wav'),
(10, 'Song 10', 'Album 10', GETUTCDATE(), 'sample1.mp3'),
(11, 'Song 11', 'Album 11', GETUTCDATE(), 'sample2.wav'),
(12, 'Song 12', 'Album 12', GETUTCDATE(), 'sample3.wav'),
(13, 'Song 13', 'Album 13', GETUTCDATE(), 'sample1.mp3'),
(14, 'Song 14', 'Album 14', GETUTCDATE(), 'sample2.wav'),
(15, 'Song 15', 'Album 15', GETUTCDATE(), 'sample3.wav'),
(16, 'Song 16', 'Album 16', GETUTCDATE(), 'sample1.mp3'),
(17, 'Song 17', 'Album 17', GETUTCDATE(), 'sample2.wav'),
(18, 'Song 18', 'Album 18', GETUTCDATE(), 'sample3.wav'),
(19, 'Song 19', 'Album 19', GETUTCDATE(), 'sample1.mp3'),
(20, 'Song 20', 'Album 20', GETUTCDATE(), 'sample2.wav');

SET IDENTITY_INSERT [Songs] OFF;
GO

-- Optional: Seed a few likes
INSERT INTO [Likes] ([SongId], [UserId], [CreatedAt]) VALUES
(1, 1, GETUTCDATE()),
(5, 1, GETUTCDATE()),
(10, 1, GETUTCDATE());
GO
